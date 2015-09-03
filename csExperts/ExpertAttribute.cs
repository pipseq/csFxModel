using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace csExperts
{
    public class ExpertAttribute : Attribute
    {
        public ExpertAttribute() { }
        public ExpertAttribute(String Description_in)
        {
            this.description = Description_in;
        }
        protected String description = "An automated trading strategy";
        public String Description
        {
            get
            {
                return this.description;
            }
        }

        public static Dictionary<string, string> getClasses()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();


            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    var attribs = type.GetCustomAttributes(typeof(ExpertAttribute), false);
                    if (attribs != null && attribs.Length > 0)
                        foreach (Attribute a in attribs)
                        {
                            ExpertAttribute expAttr = a as ExpertAttribute;
                            string aqn = type.AssemblyQualifiedName;
                            map.Add(aqn, expAttr.Description);
                            //map.Add(type.Name, expAttr.Description);
                        }
                }
            }

            return map;
        }
    }

}
