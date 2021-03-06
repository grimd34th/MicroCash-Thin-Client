﻿/*
 * MicroCash Thin Client
 * Please see License.txt for applicable copyright and licensing details.
 */

using System;
using System.Xml;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MicroCash.Client.Thin
{
    internal class ThinUser
    {
        public string m_name;
        string m_FileName;
        string m_pass1;
        string m_pass2;
        public int m_icon;
        public List<Account> m_Accounts;

        public ThinUser()
        {
            m_icon = 0;
            m_Accounts = new List<Account>();
        }
        
        public void Create(string name, string pass1, string pass2)
        {
            m_FileName = "user_" + name + ".xml";
            m_name = name;
            m_pass1 = pass1;
            m_pass2 = pass2;
        }

        public void AddNewAccount(string name)
        {
            Account newAccount = new Account();
            newAccount.Name=name;
            newAccount.GenerateKeyPair();
            m_Accounts.Add(newAccount);            
        }

        public bool Save()
        {
            XmlTextWriter writer = null; 
            
            try
            {
                writer = new XmlTextWriter(m_FileName, System.Text.Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("User");

                writer.WriteElementString("name", m_name);
                writer.WriteElementString("pass1", m_pass1);
                writer.WriteElementString("pass2", m_pass2);
                writer.WriteElementString("icon", m_icon.ToString());

                foreach(Account account in m_Accounts)
                {
                    account.AccountXMLSave(writer);
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }
            catch
            {
                MessageBox.Show("Failed to write " + m_name);
                return false;
            }
            finally
            {
                writer.Close();
            }


            return true;
        }

        public bool LoadFromFile(string filename)
        {
            m_FileName = filename;
            bool bRet = false;
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(m_FileName);
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element: // The node is an element.
                            switch (reader.Name)
                            {
                                case "name": reader.MoveToContent(); m_name = reader.ReadElementContentAsString(); break;
                                case "pass1": reader.MoveToContent(); m_pass1 = reader.ReadElementContentAsString(); break;
                                case "pass2": reader.MoveToContent(); m_pass2 = reader.ReadElementContentAsString(); break;
                                case "icon": reader.MoveToContent(); m_icon = reader.ReadElementContentAsInt(); break;
                                case "account": 
                                    {
                                        Account newAccount = new Account();
                                        if (newAccount.AccountXMLLoad(reader))
                                        {
                                            m_Accounts.Add(newAccount);
                                        }
                                        break;
                                    }
                            }
                            break;
                        case XmlNodeType.EndElement:
                            break;
                    }
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                return false;
            }
            catch (Exception)
            {
                MessageBox.Show("Error reading " + m_FileName);
            }
            finally
            {
                reader.Close();
            }
            return bRet;
        }
    }
     
}