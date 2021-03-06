﻿//The MIT License (MIT)

//Copyright (c) 2014 Richard Ranft

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace StoreManagementApp
{
    /// <summary>
    /// This class is for loading .ini files containing settings for the application.
    /// 
    /// All key/value pairs found before a [Section] has been defined are automatically
    /// assigned to the [Default] section.
    /// 
    /// If constructed with the default constructor the settings will be assumed to be in a file
    /// with the executable called "settigns.ini".
    /// 
    /// The .ini file can contain any number of [Sections] and key/value pairs.  It is up to the 
    /// developer to access the required data in the host application.
    /// </summary>
    public class CSettings
    {
        private List<String> m_iniText;
        private List<KeyValuePair<String, List<KeyValuePair<String, String>>>> m_attributeList;
        private String m_fileName;
        private bool m_dirty;
        
        /// <summary>
        /// The Configuration property should probably be removed.  For the time being I'm leaving it
        /// in to facilitate debugging - it allows the developer to access all meaningful lines of 
        /// text from the .ini file (meaning all comments are stripped).
        /// </summary>
        public List<String> Configuration
        {
            get
            {
                return m_iniText;
            }
        }

        public List<KeyValuePair<String, List<KeyValuePair<String, String>>>> Attributes
        {
            get
            {
                return m_attributeList;
            }
        }

        public CSettings()
        {
            m_fileName = "settings.ini";
            m_iniText = new List<String>();
            m_attributeList = new List<KeyValuePair<String, List<KeyValuePair<String, String>>>>();
            m_dirty = false;
        }

        public CSettings(String fileName)
        {
            m_fileName = fileName;
            m_iniText = new List<String>();
            m_attributeList = new List<KeyValuePair<String, List<KeyValuePair<String, String>>>>();
            m_dirty = false;
        }

        public List<KeyValuePair<String, String>> GetSection(String sectionName)
        {
            foreach(KeyValuePair<String, List<KeyValuePair<String, String>>> section in m_attributeList)
            {
                if(section.Key == sectionName)
                    return section.Value;
            }
            return null;
        }

        public bool LoadSettings()
        {
            bool success = false;
            try
            {
                using(StreamReader reader = new StreamReader(m_fileName))
                {
                    String line = "";
                    int index = 0;
                    
                    String section = "[Default]";
                    List<KeyValuePair<String, String>> sectionValues = new List<KeyValuePair<String, String>>();
                    KeyValuePair<String, List<KeyValuePair<String, String>>> sectionPair = new KeyValuePair<String, List<KeyValuePair<String, String>>>(section, sectionValues);
                    m_attributeList.Add(sectionPair);

                    while(!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        if (line.Contains(';')) // ini file comment
                        {
                            index = line.IndexOf(';');
                            line = line.Remove(index);
                        }
                        if (line.Length < 1)
                            continue;
                        
                        m_iniText.Add(line);

                        if (line.StartsWith("["))
                        {
                            bool sectionExists = false;
                            // check to see if this [Section] exists.  If so, add new entries to it
                            foreach(KeyValuePair<String, List<KeyValuePair<String, String>>> sectionEntry in m_attributeList)
                            {
                                if (sectionEntry.Key.ToString() == line.ToString())
                                {
                                    sectionPair = sectionEntry;
                                    sectionExists = true;
                                    continue;
                                }
                            }
                            if (sectionExists)
                                continue;
                            // otherwise, create a new [Section]
                            section = line;
                            sectionValues = new List<KeyValuePair<String, String>>();
                            sectionPair = new KeyValuePair<String, List<KeyValuePair<String, String>>>(section, sectionValues);
                            m_attributeList.Add(sectionPair);
                            continue;
                        }

                        String[] parts = line.Split('=');
                        if (parts.Length != 2)
                        {
                            Console.WriteLine("{0} : Invalid key/value pair in ini file: {1}", DateTime.Now.ToString(), line);
                            continue;
                        }

                        KeyValuePair<String, String> attribute = new KeyValuePair<String, String>(parts[0].Trim(), parts[1].Trim());

                        // check for existing attributes.  If we're trying to add the same attribute again we remove the
                        // old and add the new - so later definitions override earlier ones.
                        foreach(KeyValuePair<String, String> attr in sectionPair.Value)
                        {
                            if(attr.Key.ToString().ToLower() == parts[0].ToLower())
                            {
                                sectionPair.Value.Remove(attr);
                                break;
                            }
                        }
                        sectionPair.Value.Add(attribute);
                    }
                }

                success = true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("{0} : Exception : {1}", DateTime.Now.ToString(), ex.Message);
                success = false;
            }

            return success;
        }

        public bool SaveSettings()
        {
            if (m_dirty)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(m_fileName))
                    {
                        foreach (KeyValuePair<String, List<KeyValuePair<String, String>>> section in m_attributeList)
                        {
                            String sectionTitle = section.Key;
                            try
                            {
                                writer.WriteLine(sectionTitle);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("{0} : Exception : {1}", DateTime.Now.ToString(), e.Message);
                                return false;
                            }
                            foreach (KeyValuePair<String, String> attribute in section.Value)
                            {
                                try
                                {
                                    writer.WriteLine(attribute.Key.ToUpper() + "=" + attribute.Value.ToUpper());
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("{0} : Exception : {1}", DateTime.Now.ToString(), e.Message);
                                    return false;
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("{0} : Exception : {1}", DateTime.Now.ToString(), ex.Message);
                    return false;
                }
                m_dirty = false;
            }
            return true;
        }

        public bool Set(String sectionName, String key, String value)
        {
            if (sectionName.Equals(""))
                sectionName = "[Default]";

            foreach (KeyValuePair<String, List<KeyValuePair<String, String>>> section in m_attributeList)
            {
                if(sectionName.Equals(section.Key))
                {
                    KeyValuePair<String, String> attribute = new KeyValuePair<String, String>(key, value);

                    // check for existing attributes.  If we're trying to add the same attribute again we remove the
                    // old and add the new - so later definitions override earlier ones.
                    foreach (KeyValuePair<String, String> attr in section.Value)
                    {
                        if (attr.Key.ToString().ToLower() == key.ToLower())
                        {
                            if (attr.Value.ToLower() != value.ToLower())
                                m_dirty = true;
                            section.Value.Remove(attr);
                            break;
                        }
                    }
                    section.Value.Add(attribute);
                    m_dirty = true;
                }
            }
            if(!m_dirty)
            {
                KeyValuePair<String, List<KeyValuePair<String, String>>> newSection = new KeyValuePair<String, List<KeyValuePair<String, String>>>(sectionName, new List<KeyValuePair<string,string>>());
                KeyValuePair<String, String> newAttribute = new KeyValuePair<String, String>(key, value);
                m_attributeList.Add(newSection);
                newSection.Value.Add(newAttribute);
                m_dirty = true;
            }
            return true;
        }
    }
}
