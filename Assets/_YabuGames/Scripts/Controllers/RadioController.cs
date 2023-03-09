using System;
using _YabuGames.Scripts.Interfaces;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;


namespace _YabuGames.Scripts.Controllers
{
    public class RadioController : MonoBehaviour,IMergeable
    {

        public int radioLevel = 1;
        public float rangeMultiplier = 1.5f;

        [SerializeField] private GameObject[] radios;
        
        private Vector3 _defaultScale;
        
        private void OnDestroy()
        {
            transform.DOKill();
        }

        void Start()
        {
            _defaultScale = transform.localScale;
        }

        private void SpawnNewRadio()
        {
            var radio = Instantiate(radios[radioLevel]);
            radio.transform.SetPositionAndRotation(transform.position, transform.rotation);
            
            var effectScale = radio.transform.localScale + Vector3.one*.3f;
            radio.transform.DOScale(effectScale, .5f).SetEase(Ease.InSine).SetLoops(2, LoopType.Yoyo);
        }
        public void Merge(RadioController radio)
        {
            var equal = radioLevel == radio.radioLevel;
            if(!equal) return;
            
            SpawnNewRadio();
            transform.DOScale(Vector3.zero, .5f).SetEase(Ease.InBack);
            Destroy(gameObject,.5f);
        }

        public void HoldingEffect()
        {
            transform.DOKill();
            var effectScale = _defaultScale + Vector3.one * .5f;
            transform.DOScale(effectScale, .5f).SetEase(Ease.InSine);
        }

        public void Disappear()
        {
            transform.DOScale(Vector3.zero, .5f).SetEase(Ease.InBack);
            Destroy(gameObject,.5f);
        }

        public void ReleaseEffect()
        {
            transform.DOKill();
            transform.DOScale(_defaultScale, .5f).SetEase(Ease.OutBack);

        }
    }
}
