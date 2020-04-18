using System.Collections.Generic;
using Tofunaut.Core;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeepr.Game
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        public const string GenSeedKey = "gen_seed";

        public static GameCamera Camera { get; private set; }
        public static bool IsGenerating => _instance._isGenerating;

        public Floor CurrentFloor => _floors[_currentLevel];

        private List<Actor> _actors;
        private List<Floor> _floors;
        private Actor _playerActor;
        private int _currentLevel;
        private int _seed;
        private bool _isGenerating;

        private void Start()
        {
            _seed = PlayerPrefs.GetInt(GenSeedKey, UnityEngine.Random.Range(int.MinValue, int.MaxValue));
            PlayerPrefs.SetInt(GenSeedKey, _seed);

            Debug.Log($"using seed {_seed}");

            Camera = new GameCamera();
            Camera.Render(transform);

            _actors = new List<Actor>();
            _floors = new List<Floor>();

            _playerActor = CreateActor(AppManager.AssetManager.Get<ActorConfigAsset>("PlayerActorConfig").data, new IntVector2(0, 0));
            Camera.SetTarget(_playerActor.GameObject);
            PlayerActorInputController.Instance.currentTarget = _playerActor;

            EnterLevel(0);
        }

        private void Update()
        {
            //if(!IsGenerating)
            //{
            //    if (Input.GetKeyDown(KeyCode.N))
            //    {
            //        EnterLevel(_currentLevel + 1);
            //    }
            //    if (_currentLevel > 0 && Input.GetKeyDown(KeyCode.B))
            //    {
            //        EnterLevel(_currentLevel - 1);
            //    }
            //}
        }

        private void EnterLevel(int level)
        {
            Floor previousFloor = null;
            if(_currentLevel >= 0 && _currentLevel < _floors.Count)
            {
                previousFloor = _floors[_currentLevel];
            }

            _currentLevel = level;

            void SwitchPlayerToNextFloor(Floor floor)
            {
                if (previousFloor != null)
                {
                    previousFloor.RemoveActor(_playerActor);
                    previousFloor.GameObject.SetActive(false);
                }
                floor.AddActor(_playerActor, floor.ladderUp.coord);
                Camera.SnapToTarget();
            }

            if (_currentLevel >= 0 && _currentLevel < _floors.Count)
            {
                // the floor already exists, so make sure it's built and active
                Floor floor = _floors[_currentLevel];
                if (floor.IsBuilt)
                {
                    floor.GameObject.SetActive(true);
                }
                else
                {
                    floor.Render(transform);
                }

                SwitchPlayerToNextFloor(floor);
            }
            else
            {
                _isGenerating = true;

                // the floor doesn't exist, so generate it
                FloorGen.GenerateFloor(_seed, level, (Floor payload) =>
                {
                    _isGenerating = false;

                    _floors.Add(payload);
                    payload.Render(transform);

                    SwitchPlayerToNextFloor(payload);
                });
            }
        }

        public Actor CreateActor(ActorConfig config, IntVector2 position)
        {
            Actor actor = new Actor(this, config);
            actor.LocalPosition = position.ToUnityVector3_XY();
            _actors.Add(actor);

            actor.Render(transform);

            return actor;
        }

        public static void GoToUpperFloor()
        {
            if(IsGenerating)
            {
                return;
            }

            if (_instance._currentLevel > 0)
            {
                _instance.EnterLevel(_instance._currentLevel - 1);
            }
        }

        public static void GoToLowerFloor()
        {
            if (IsGenerating)
            {
                return;
            }

            _instance.EnterLevel(_instance._currentLevel + 1);
        }
    }
}