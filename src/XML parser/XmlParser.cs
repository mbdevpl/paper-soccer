using System;
using System.Security.Policy;
using System.Text;
using System.Xml;
using Objects;
using System.Xml.Serialization;
using System.IO;
static public class XmlParser
{
    /// <summary>
    /// Deparse a message to an XML string
    /// </summary>
    /// <param name="mes">Message to be deparsed</param>
    /// <returns>String representation of deparsed message</returns>
    public static string Deparse(message mes)
    {
        if (!(Validate(mes)))
            throw (new PolicyException("Message does not follow the protocol"));
        XmlSerializer serializer = new XmlSerializer(typeof(message));
        MemoryStream stream = new MemoryStream();
        XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
        xsn.Add(String.Empty, String.Empty);
        XmlTextWriter sw = new XmlTextWriter(stream, Encoding.UTF8);
        serializer.Serialize(sw, mes, xsn);
        return Encoding.UTF8.GetString(stream.ToArray()).Substring(1);

    }
    /// <summary>
    /// Parse a message from appropriate xml-like string
    /// </summary>
    /// <param name="xml">String representation of the message in utf-8</param>
    /// <returns>Message object parsed from the string</returns>
    public static message Parse(string xml)
    {
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        XmlSerializer serializer = new XmlSerializer(typeof(message));
        XmlReader reader = XmlReader.Create(new StreamReader(stream, Encoding.UTF8));
        message mes = (message)serializer.Deserialize(reader);
        if (!(Validate(mes)))
            throw (new PolicyException("Message does not follow the protocol"));
        return mes;
    }
    /// <summary>
    /// Validation checking content-specific restrictions, like max>min etc.
    /// </summary>
    /// <param name="mes">Message object to be validated</param>
    /// <returns>True if Ok, false otherwise</returns>
    private static bool Validate(message mes)
    {
        if (mes._gameMasterLogin != null)
            if (mes._gameMasterLogin._playersMin < 1 || mes._gameMasterLogin._playersMax < mes._gameMasterLogin._playersMin)
                return false;
        return true;
    }
    /// <summary>
    /// This method will collect the whole message from the StreamReader and parse it into an object
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static message listen(StreamReader reader)
    {
        string msg = "";
        while (!msg.Contains("/message>") && !msg.Contains("thankYou\" />") && !msg.Contains("leaveGame\" />"))
        {
            var c = (char)reader.Read();
            if (c != '\n' && c != '\r')
                msg += c;
        }
        return XmlParser.Parse(msg);
    }
}