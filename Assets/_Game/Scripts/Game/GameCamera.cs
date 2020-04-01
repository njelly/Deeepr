using Tofunaut.Animation;
using Tofunaut.SharpUnity;
using UnityEngine;

namespace Tofunaut.Deeepr.Game
{
    public class GameCamera : SharpCamera
    {
        public const float MoveToTargetTime = 1.5f;

        private GameObject _target;
        private Vector3 _targetPos;

        private TofuAnimation _followAnimation;

        public GameCamera() : base("GameCamera")
        {
            _cameraOrthographic = true;
            _cameraOrthographicSize = 10;
            _cameraBackgroundColor = AppManager.ColorPallet.Get(ColorPallet.EColor.Black);

            LocalPosition = new Vector3(0, 0, -10);
        }

        public void SetTarget(GameObject target)
        {
            _target = target;

            Vector3 startPos = Transform.position;

            _followAnimation?.Stop();
            _followAnimation = new TofuAnimation()
                .Value01(MoveToTargetTime, EEaseType.EaseOutExpo, (float newValue) =>
                {
                    if (_target)
                    {
                        _targetPos = new Vector3(_target.transform.position.x, _target.transform.position.y, -10f);
                    }

                    Transform.position = Vector3.LerpUnclamped(startPos, _targetPos, newValue);
                })
                .Then()
                .WaitUntil(() => 
                { 
                    if(_target)
                    {
                        _targetPos = new Vector3(_target.transform.position.x, _target.transform.position.y, -10f);
                    }
                    Transform.position = _targetPos;
                    return !_target; 
                })
                .Play();
        }
    }
}