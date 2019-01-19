using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace XmlReadExample
{
  /// <summary>
  /// Пример к статье «Обработка больших XML при помощи XmlReader»
  /// Постоянный адрес статьи: 
  /// http://kbyte.ru/ru/Programming/Articles.aspx?id=74&mode=art
  /// Автор: Алексей Немиро
  /// 12 августа 2012 года
  /// http://aleksey.nemiro.ru
  /// http://kbyte.ru
  /// </summary>
  class Program
  {

    static void Main(string[] args)
    {

      // ReadByXmlDocument("small.xml");
      // ReadByXPathDocument("small.xml");
      // ReadByXmlReader0("small.xml");
      // ReadByXmlReader1("small.xml");
      // ReadByXmlReader2("small.xml");
      // ReadByXmlReader3("small.xml");
      // ReadByXmlReader4("small.xml");
      // ReadByXmlReader5("small2.xml");

      // Чтение большого xml-файла при помощи XmlDocument
      // Приложение будет потреблять от ста мегабайт ОЗУ
      // ReadByXmlDocument("large2.xml");

      // Чтение большого xml-файла при помощи XmlReader
      // Приложение будет потреблять не более десяти мегабайт ОЗУ
      // ReadByXmlReader3("large.xml");
      ReadByXmlReader5("large2.xml");

      Console.ReadKey();
    }

    static void ReadByXmlDocument(string path)
    {
      XmlDocument xml = new XmlDocument();
      xml.Load(path);
      foreach (XmlNode n in xml.SelectNodes("/kbyte/members/member"))
      {
        Console.WriteLine("KUID: {0}", n.Attributes["kuid"].Value);
        Console.WriteLine("Псевдоним: {0}", n.SelectSingleNode("nickname").InnerText);
        Console.WriteLine("Имя: {0}", n.SelectSingleNode("firstName").InnerText);
        Console.WriteLine("Фамилия: {0}", n.SelectSingleNode("lastName").InnerText);
        Console.WriteLine("------------------------------------------");
      }
    }

    static void ReadByXPathDocument(string path)
    {
      XPathDocument xml = new XPathDocument(path);
      XPathNavigator nav = xml.CreateNavigator();
      foreach (XPathNavigator n in nav.Select("/kbyte/members/member"))
      {
        Console.WriteLine("KUID: {0}", n.GetAttribute("kuid", ""));
        Console.WriteLine("Псевдоним: {0}", n.SelectSingleNode("nickname").Value);
        Console.WriteLine("Имя: {0}", n.SelectSingleNode("firstName").Value);
        Console.WriteLine("Фамилия: {0}", n.SelectSingleNode("lastName").Value);
        Console.WriteLine("------------------------------------------");
      }
    }

    static void ReadByXmlReader0(string path)
    {
      using (XmlReader xml = XmlReader.Create(path))
      {
        while (xml.Read())
        {
          Console.WriteLine("Тип узла: {0}", xml.NodeType);
          Console.WriteLine("Имя узла: {0}", xml.Name);
          Console.WriteLine("Значение узла: {0}", xml.Value);
        }
      }
    }

    static void ReadByXmlReader1(string path)
    {
      string lastNodeName = "";
      using (XmlReader xml = XmlReader.Create(path))
      {
        while (xml.Read())
        {
          switch (xml.NodeType)
          {
            case XmlNodeType.Element:
              // нашли элемент member
              if (xml.Name == "member") 
              {
                if (xml.HasAttributes)
                {
                  // поиск атрибута kuid
                  while (xml.MoveToNextAttribute())
                  {
                    if (xml.Name == "kuid")
                    {
                      Console.WriteLine("KUID: {0}", xml.Value);
                      break;
                    }
                  }
                }
              }

              // запоминаем имя найденного элемента
              lastNodeName = xml.Name;

              break;

            case XmlNodeType.Text:
              // нашли текст, смотрим по имени элемента, что это за текст
              if (lastNodeName == "nickname")
              {
                Console.WriteLine("Псевдоним: {0}", xml.Value);
              }
              else if (lastNodeName == "firstName")
              {
                Console.WriteLine("Имя: {0}", xml.Value);
              }
              else if (lastNodeName == "lastName")
              {
                Console.WriteLine("Фамилия: {0}", xml.Value);
              }
              break;

            case XmlNodeType.EndElement:
              // закрывающий элемент
              if (xml.Name == "member")
              {
                Console.WriteLine("------------------------------------------");
              }
              break;
          }
        }
      }
    }
    
    static void ReadByXmlReader2(string path)
    {
      using (XmlReader xml = XmlReader.Create(path))
      {
        while (xml.Read())
        {
          switch (xml.NodeType)
          {
            case XmlNodeType.Element:
              // нашли элемент member
              if (xml.Name == "member")
              {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml.ReadOuterXml());
                XmlNode n = xmlDoc.SelectSingleNode("member");
                Console.WriteLine("KUID: {0}", n.Attributes["kuid"].Value);
                Console.WriteLine("Псевдоним: {0}", n.SelectSingleNode("nickname").InnerText);
                Console.WriteLine("Имя: {0}", n.SelectSingleNode("firstName").InnerText);
                Console.WriteLine("Фамилия: {0}", n.SelectSingleNode("lastName").InnerText);
                Console.WriteLine("------------------------------------------");
              }
              break;
          }
        }
      }
    }

    static void ReadByXmlReader3(string path)
    {
      using (XmlReader xml = XmlReader.Create(path))
      {
        while (xml.Read())
        {
          switch (xml.NodeType)
          {
            case XmlNodeType.Element:
              // нашли элемент member
              if (xml.Name == "member")
              {
                Member m = new Member(xml.ReadOuterXml());
                Console.WriteLine("KUID: {0}", m.KUID);
                Console.WriteLine("Псевдоним: {0}", m.Nickname);
                Console.WriteLine("Имя: {0}", m.FirstName);
                Console.WriteLine("Фамилия: {0}", m.LastName);
                Console.WriteLine("------------------------------------------");
              }
              break;
          }
        }
      }
    }

    static void ReadByXmlReader4(string path)
    {
      StringBuilder sb = null;
      XmlWriter w = null;
      XmlWriterSettings ws = new XmlWriterSettings() { Encoding = Encoding.UTF8 };

      bool isMember = false, isSkiped = false;

      using (XmlReader xml = XmlReader.Create(path))
      {
        while (xml.Read())
        {
          switch (xml.NodeType)
          {
            case XmlNodeType.Element:
              // нашли элемент member
              if (xml.Name == "member")
              {
                isMember = true;

                if (sb != null)
                {
                  w.Flush();
                  w.Close();

                  XmlDocument xmlDoc = new XmlDocument();
                  xmlDoc.LoadXml(sb.ToString());
                  XmlNode n = xmlDoc.SelectSingleNode("member");
                  Console.WriteLine("KUID: {0}", n.Attributes["kuid"].Value);
                  Console.WriteLine("Псевдоним: {0}", n.SelectSingleNode("nickname").InnerText);
                  Console.WriteLine("Имя: {0}", n.SelectSingleNode("firstName").InnerText);
                  Console.WriteLine("Фамилия: {0}", n.SelectSingleNode("lastName").InnerText);
                  Console.WriteLine("------------------------------------------");
                }

                sb = new StringBuilder();
                w = XmlWriter.Create(sb, ws);
                w.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\"");
              }
              else if (xml.Name == "messages")
              {
                // пропускаем фрагмент с сообщениями
                xml.Skip();
                // ставим отметку, что фрагмент пропущен
                isSkiped = true;
              }

              // это данные пользователя и не пропущенный фрагмент
              if (isMember && !isSkiped)
              {
                w.WriteStartElement(xml.Name);
                // если есть атрибуты, записываем их
                if (xml.HasAttributes)
                {
                  while (xml.MoveToNextAttribute())
                  {
                    w.WriteAttributeString(xml.Name, xml.Value);
                  }
                }
              }

              // убираем отметку о пропущенном фрагменте
              isSkiped = false;
              break;

            case XmlNodeType.Text:
              if (isMember)
              {
                w.WriteString(xml.Value);
              }
              break;

            case XmlNodeType.EndElement:
              if (isMember)
              {
                w.WriteFullEndElement();
              }
              if (xml.Name == "member")
              {
                isMember = false;
              }
              break;

          }
        }
      }
    }

    static void ReadByXmlReader5(string path)
    {
      using (XmlReader xml = XmlReader.Create(path))
      {

        StringBuilder sb = null;
        XmlWriter w = null;
        XmlWriterSettings ws = new XmlWriterSettings() { Encoding = Encoding.UTF8 };

        bool isMember = false, isSkiped = false;

        while (xml.Read())
        {
          switch (xml.NodeType)
          {
            case XmlNodeType.Element:
              // нашли элемент member
              if (xml.Name == "member")
              {
                isMember = true;

                if (sb != null)
                {
                  w.Flush();
                  w.Close();

                  XmlDocument xmlDoc = new XmlDocument();
                  xmlDoc.LoadXml(sb.ToString());
                  XmlNode n = xmlDoc.SelectSingleNode("member");
                  Console.WriteLine("KUID: {0}", n.Attributes["kuid"].Value);
                  Console.WriteLine("Псевдоним: {0}", n.SelectSingleNode("nickname").InnerText);
                  Console.WriteLine("Имя: {0}", n.SelectSingleNode("firstName").InnerText);
                  Console.WriteLine("Фамилия: {0}", n.SelectSingleNode("lastName").InnerText);
                  Console.WriteLine("------------------------------------------");
                }

                sb = new StringBuilder();
                w = XmlWriter.Create(sb, ws);
                w.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\"");
              }
              // нашли элемент messages
              else if (xml.Name == "messages")
              {
                // создаем XmlReader для узла messages и обрабатываем его
                ReadSubtree(xml.ReadSubtree());

                // пропускаем фрагмент с сообщениями
                xml.Skip();
                // ставим отметку, что фрагмент пропущен
                isSkiped = true;
              }

              // это данные пользователя
              if (isMember && !isSkiped)
              {
                w.WriteStartElement(xml.Name);
                if (xml.HasAttributes)
                {
                  while (xml.MoveToNextAttribute())
                  {
                    w.WriteAttributeString(xml.Name, xml.Value);
                  }
                }
              }

              // убираем отметку о пропущенном фрагменте
              isSkiped = false;
              break;

            case XmlNodeType.Text:
              if (isMember)
              {
                w.WriteString(xml.Value);
              }
              break;

            case XmlNodeType.EndElement:
              if (isMember)
              {
                w.WriteFullEndElement();
              }
              // достигнут конец элемента member 
              if (xml.Name == "member")
              {
                isMember = false;
              }
              break;

          }
        }
      }
    }
    static void ReadSubtree(XmlReader xml)
    {
      while (xml.Read())
      {
        switch (xml.NodeType)
        {
          case XmlNodeType.Element:
            // нашли элемент message
            if (xml.Name == "message")
            {
              XmlDocument xmlDoc = new XmlDocument();
              xmlDoc.LoadXml(xml.ReadOuterXml());
              XmlNode n = xmlDoc.SelectSingleNode("message");
              Console.WriteLine("Сообщение # {0}", n.Attributes["id"].Value);
              Console.WriteLine("Тема: {0}", n.SelectSingleNode("subject").InnerText);
              Console.WriteLine("Текст: {0}", n.SelectSingleNode("text").InnerText);
              Console.WriteLine("------------------------------------------");
            }
            break;
        }
      }
    }

  }
}
