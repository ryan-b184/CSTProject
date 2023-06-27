using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

/// <summary>
/// PURPOSE: This is essentially a wrapper class for a basic data list.
/// Adds additional metadata and structure so that our <InputDataListSelect> component can use the data list,
/// and so Blazor can handle it under the hood.
/// 
/// Jonathan Cruz (CST224)
/// John Sierra (CST213)
/// </summary>
namespace CSTScheduling.Data.Models
{

    public static class BlazorDataList<TValue>
    {

        public static Dictionary<int, TValue> TValueObject
        {
            get
            {
                Dictionary<int, TValue> list = new Dictionary<int, TValue>();
                foreach (var v in TValueList)
                {
                    Type t = v.GetType(); // get the type of the generic
                    PropertyInfo[] props = t.GetProperties(); // get all properties of the class
                    int z = (int)props[0].GetValue(v); // first property should be ID
                    list.Add(z, v);
                }
                return list;
            }
        }

        public static Dictionary<int, string> TValueDictionary
        {
            get
            {
                Dictionary<int, string> list = new Dictionary<int, string>();

                if(TValueList != null)
                {       
                    foreach (var v in TValueList)
                    {
                        Type t = v.GetType(); // get the type of the generic
                        PropertyInfo[] props = t.GetProperties(); // get all properties of the class
                        int z =(int) props[0].GetValue(v); // first property should be ID
                        list.Add(z, v.ToString()); 
 
                    }

                    
                }
                return list;
            }
        }

        /// <summary>
        /// This property must be assigned the data array (Room, Instructor, etc.).
        /// Assign this on the page that is using the <InputDataListSelect> component.
        /// </summary>
        public static List<TValue> TValueList { get; set; }
    }
}
