using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ACEOCustomBuildables
{
    public static class ListHelper
    {
        public static void DestroyResetList(this List<GameObject> listofGameobjects)
        {
            foreach (GameObject gameObject in listofGameobjects)
            {
                if (gameObject == null)
                {
                    continue;
                }
                GameObject.Destroy(gameObject);
            }
            listofGameobjects = new List<GameObject>();
        }
    }
}
