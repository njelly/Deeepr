using System.Collections.Generic;
using Tofunaut.Core;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeepr.Game
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        public static GameCamera Camera { get; private set; }

        private List<Actor> _actors;
        private List<Floor> _floors;
        private Actor _playerActor;
        private int _currentLevel;

        private void Start()
        {
            Camera = new GameCamera();
            Camera.Render(transform);

            _actors = new List<Actor>();
            _floors = new List<Floor>();

            _floors.Add(Floor.TestFloor());

            _playerActor = CreateActor(AppManager.AssetManager.Get<ActorConfigAsset>("PlayerActorConfig").data, new IntVector2(5, 5));
            PlayerActorInputController.Instance.currentTarget = _playerActor;

            Camera.SetTarget(_playerActor.GameObject);

            EnterLevel(0);
        }

        private void EnterLevel(int level)
        {
            _currentLevel = level;

            for(int i = 0; i < _floors.Count; i++)
            {
                if(i == _currentLevel)
                {
                    if(!_floors[i].IsBuilt)
                    {
                        _floors[i].Render(transform);
                    }
                }
                else
                {
                    if (_floors[i].IsBuilt)
                    {
                        _floors[i].GameObject.SetActive(false);
                    }
                }
            }
        }

        private void Update()
        {
            
        }

        public Actor CreateActor(ActorConfig config, IntVector2 position)
        {
            Actor actor = new Actor(this, config);
            actor.LocalPosition = position.ToUnityVector3_XY();
            _actors.Add(actor);

            actor.Render(transform);

            return actor;
        }
    }
}