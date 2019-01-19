using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace XmlReadExample
{
  [XmlRoot("member")]
  public class Member
  {

    [XmlAttribute("kuid")]
    public int KUID { get; set; }
    [XmlElement("nickname")]
    public string Nickname { get; set; }
    [XmlElement("firstName")]
    public string FirstName { get; set; }
    [XmlElement("lastName")]
    public string LastName { get; set; }

    public Member() {}

    public Member(string xml)
    {
      LoadXml(xml);
    }

    public void LoadXml(string source)
    {
      XmlSerializer mySerializer = new XmlSerializer(this.GetType());
      using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(source)))
      {
        object obj = mySerializer.Deserialize(ms);
        foreach (PropertyInfo p in obj.GetType().GetProperties())
        {
          PropertyInfo p2 = this.GetType().GetProperty(p.Name);
          if (p2 != null && p2.CanWrite)
          {
            p2.SetValue(this, p.GetValue(obj, null), null);
          }
        }
      }
    }

  }
}
