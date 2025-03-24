using UnityEngine;
using Utility;

namespace Global
{
    public class GlobalResources : SingletonBehaviour<GlobalResources>
    {
        public override void Instantiate()
        {
            
        }

        [SerializeField] private DataBehavioursManager _dataBehaviours;

        public DataBehavioursManager DataBehaviours => _dataBehaviours;
    }
}
