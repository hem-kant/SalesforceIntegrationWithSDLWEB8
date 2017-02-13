using System.Xml.Serialization;

namespace SalesforceIntegrationWithSDLWeb8.DAL.Model
{
    [XmlRoot(ElementName = "fName", Namespace = "http://www.sdl.com/web/schemas/core")]
    public class FName
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "lName", Namespace = "http://www.sdl.com/web/schemas/core")]
    public class LName
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "emaiID", Namespace = "http://www.sdl.com/web/schemas/core")]
    public class EmaiID
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "company", Namespace = "http://www.sdl.com/web/schemas/core")]
    public class Company
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Lead", Namespace = "http://www.sdl.com/web/schemas/core")]
    public class Lead
    {
        private FName f;
        private LName l;
        private Company c;

        public Lead(FName f)
        {
            this.f = f;
        }

        public Lead(LName l)
        {
            this.l = l;
        }

        public Lead(Company c)
        {
            this.c = c;
        }

        public Lead()
        {
        }

        [XmlElement(ElementName = "fName", Namespace = "http://www.sdl.com/web/schemas/core")]
        public FName FName { get; set; }
        [XmlElement(ElementName = "lName", Namespace = "http://www.sdl.com/web/schemas/core")]
        public LName LName { get; set; }
        [XmlElement(ElementName = "emaiID", Namespace = "http://www.sdl.com/web/schemas/core")]
        public EmaiID EmaiID { get; set; }
        [XmlElement(ElementName = "company", Namespace = "http://www.sdl.com/web/schemas/core")]
        public Company Company { get; set; }
        [XmlElement(ElementName = "sfID", Namespace = "http://www.sdl.com/web/schemas/core")]
        public string SfID { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}
