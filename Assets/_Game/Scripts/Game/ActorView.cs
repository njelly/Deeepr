using System;
using Tofunaut.Animation;
using Tofunaut.Core;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeepr.Game
{
    public class ActorView : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer _spriteRenderer;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected SpriteRenderer _interactionOffsetView;

        [Header("Animation")]
        [SerializeField] protected float _interactOffsetRotateTime;
        [SerializeField] protected float _interactFlashTime;

        public bool Initialized => _actor != null;

        protected Actor _actor;
        protected IntVector2 _previousInteractionOffset = IntVector2.Zero;

        private TofuAnimation _interactOffsetViewAnim;
        private TofuAnimation _interactFlashAnim;

        protected virtual void Update()
        {
            if(!Initialized)
            {
                return;
            }

            _spriteRenderer.flipX = _actor.Facing == Actor.EFacing.Left;

            if(!_actor.InteractOffset.Equals(_previousInteractionOffset))
            {
                UpdateInteractOffsetView();
            }
        }

        public void Initialize(Actor actor)
        {
            if(Initialized)
            {
                Debug.LogError("this ActorView is already initialized!");
                return;
            }

            _actor = actor;
            _actor.InteractionBegan += OnInteractionBegan;
            _actor.InteractionEnded += OnInteractionEnded;
        }

        private void UpdateInteractOffsetView()
        {
            if(_interactOffsetViewAnim != null)
            {
                _interactOffsetViewAnim.Stop();
            }

            Quaternion targetRot = _actor.InteractOffset.ToRotation();
            Quaternion startRot = _previousInteractionOffset.ToRotation();
            _interactOffsetViewAnim = new TofuAnimation()
                .Value01(_interactOffsetRotateTime, EEaseType.EaseOutExpo, (float newValue) =>
                {
                    _interactionOffsetView.transform.localPosition = Quaternion.LerpUnclamped(startRot, targetRot, newValue) * Vector3.right;
                })
                .Play();

            _previousInteractionOffset = _actor.InteractOffset;
        }

        private void OnInteractionBegan(object sender, EventArgs e)
        {
            AnimateColor(AppManager.ColorPallet.Get(ColorPallet.EColor.LightGray, ColorPallet.EAlpha.AlmostSolid));
        }

        private void OnInteractionEnded(object sender, EventArgs e)
        {
            AnimateColor(AppManager.ColorPallet.Get(ColorPallet.EColor.LightGray, ColorPallet.EAlpha.Faint));
        }

        private void AnimateColor(Color targetColor)
        {
            Color startColor = _interactionOffsetView.color;

            if (_interactFlashAnim != null)
            {
                _interactFlashAnim.Stop();
            }
            _interactFlashAnim = new TofuAnimation()
                .Value01(_interactFlashTime, EEaseType.Linear, (float newValue) =>
                {
                    _interactionOffsetView.color = Color.Lerp(startColor, targetColor, newValue);
                })
                .Then()
                .Execute(() =>
                {
                    _interactFlashAnim = null;
                })
                .Play();
        }
    }
}