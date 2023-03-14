using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _YabuGames.Scripts.Managers
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance;
        
    [Header("                               // Set Particles Stop Action To DISABLE //")]
    [Space(20)]
        [SerializeField] private List<GameObject> moneyParticle = new List<GameObject>();
        [SerializeField] private List<GameObject> zoneParticle = new List<GameObject>();
        [SerializeField] private List<GameObject> thirdParticle = new List<GameObject>();
        [SerializeField] private List<GameObject> fourthParticle = new List<GameObject>();

        
        private void Awake()
        {
            #region Singleton

            if (Instance != this && Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            #endregion
            
        }

        public void GetMoneyParticle(Vector3 desiredPos, int value)
        {
            var temp = moneyParticle[0];
            temp.transform.localScale = Vector3.one*.6f;
            moneyParticle.Remove(temp);
            temp.transform.position = desiredPos;
            if (temp.TryGetComponent(out TextMeshPro tmp))
            {
                tmp.text = "$" + value;
            }
            temp.SetActive(true);
            temp.transform.DOMoveY(3, 1).SetEase(Ease.InSine).OnComplete(() => Disable(temp));
            temp.transform.DOScale(Vector3.zero, .9f).SetEase(Ease.InSine);      
            moneyParticle.Add(temp);
            
        }

        private void Disable(GameObject temp)
        {
            temp.SetActive(false);
        }

        public void GetZoneParticle(Vector3 desiredPos,float size)
        {
            var temp = zoneParticle[0];
            zoneParticle.Remove(temp);
            temp.transform.position = desiredPos;
            temp.transform.localScale = Vector3.zero;
            temp.SetActive(true);
            zoneParticle.Add(temp);
            temp.transform.DOScale(Vector3.one * size, 4);
        }
        public void GetThirdParticle(Vector3 desiredPos)
        {
            var temp = thirdParticle[0];
            thirdParticle.Remove(temp);
            temp.transform.position = desiredPos;
            temp.SetActive(true);
            thirdParticle.Add(temp);
        }
        public void GetFourthParticle(Vector3 desiredPos)
        {
            var temp = fourthParticle[0];
            fourthParticle.Remove(temp);
            temp.transform.position = desiredPos;
            temp.SetActive(true);
            fourthParticle.Add(temp);
        }
    }
}
