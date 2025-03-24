using System;
using Input;
using UI;
using UnityEngine;
using Utility;

namespace Global
{
    public class MainEntryPoint : SingletonBehaviour<MainEntryPoint>
    {
        [SerializeField] private UIRoot _uiRootPrefab;
        [SerializeField] private InputController _inputControllerPrefab;
        [SerializeField] private GlobalResources _globalResources;
        [SerializeField] private AllRegistries _allRegistries;
        
        private InputController _inputControllerInstance;
        private UIRoot _uiRootInstance;
        private AllRegistries _allRegistriesInstance;

        public override void Instantiate()
        {
            DontDestroyOnLoad(gameObject);

            if (!_globalResources)
            {
                throw new ArgumentNullException(nameof(_globalResources));
            }
            
            if (!_uiRootPrefab)
            {
                throw new ArgumentNullException(nameof(_uiRootPrefab));
            }

            if (!_inputControllerPrefab)
            {
                throw new ArgumentNullException(nameof(_inputControllerPrefab));
            }

            if (!_allRegistries)
            {
                throw new ArgumentNullException(nameof(_allRegistries));
            }

            _inputControllerInstance = Instantiate(_inputControllerPrefab);
            var inputGameObject = _inputControllerInstance.gameObject;
            inputGameObject.name = "[Input Controller]";
            DontDestroyOnLoad(inputGameObject);
            
            var globalResources = Instantiate(_globalResources);
            globalResources.name = "[Global Resources]";
            DontDestroyOnLoad(globalResources);

            _uiRootInstance = Instantiate(_uiRootPrefab);
            var uiGameObject = _uiRootInstance.gameObject;
            uiGameObject.name = "[Game UI Root]";
            DontDestroyOnLoad(uiGameObject);
            
            _allRegistriesInstance = Instantiate(_allRegistries);
            var allRegistriesObject = _allRegistries.gameObject;
            allRegistriesObject.name = "[Registries]";
            DontDestroyOnLoad(allRegistriesObject);
            
            _uiRootInstance.StartGame(_inputControllerInstance);
        }
    }
}
