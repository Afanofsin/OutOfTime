using System;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private Image healthTop;
    [SerializeField] private Image skillImage;
    [SerializeField] private Image skillRing;
    [SerializeField] private TextMeshProUGUI bandagesText;
    [SerializeField] private Image bandageChains;
    
    [SerializeField] private Image weaponSlot0;
    [SerializeField] private Image weaponSlot1;  
    [SerializeField] private Image weaponSlot2; 
    
    [SerializeField] private Player _player;

    private Tween _costTweenMove;
    
    private void Start()
    {
        PlayerController.Instance.onPlayerSpawned += BindPlayer;
    }
    
    private void OnDisable()
    {
        PlayerController.Instance.onPlayerSpawned -= BindPlayer;

        if (_player != null)
        {
            _player.onHealthChanged -= OnHealthChanged;
            _player.onSkillChanged -= OnSkillChanged;
            _player.onBandageChange -= OnBandagesChanged;
            _player.onBandageUsed -= OnBandageUsed;
        }
    }
    
    private Inventory _inventory;

    private void BindPlayer(Player player)
    {
        if (_player != null)
        {
            _player.onHealthChanged -= OnHealthChanged;
            _player.onSkillChanged -= OnSkillChanged;
            _player.onBandageChange -= OnBandagesChanged;
            _player.onBandageUsed -= OnBandageUsed;

            if (_inventory != null)
                _inventory.OnInventoryChanged -= RefreshWeaponsUI;
        }

        _player = player;
        _inventory = player.Inventory;

        _player.onHealthChanged += OnHealthChanged;
        _player.onSkillChanged += OnSkillChanged;
        _player.onBandageChange += OnBandagesChanged;
        _player.onBandageUsed += OnBandageUsed;

        _inventory.OnInventoryChanged += RefreshWeaponsUI;

        OnHealthChanged(_player.CurrentHealth);
        RefreshWeaponsUI();
    }

    private void OnHealthChanged(float health)
    {
        var normalized = health / _player.MaxHealth;
        healthFill.fillAmount = normalized;

        var y = Mathf.Lerp(-90.5f, 5f, normalized) + 59f;
        healthTop.rectTransform.localPosition = new Vector3(0f, y, 0f);

        if (_player.HeldSkill == null)
        {
            skillRing.gameObject.SetActive(false);
            return;
        }
        
        CheckCost();
    }

    private void CheckCost()
    {
        var cost = (_player.CurrentHealth - _player.HeldSkill.Cost) / 100f;
        var costY = Mathf.Lerp(-90.5f, 5f, cost) + 59f;

        if (!_player.isSkilled)
        {
            skillRing.gameObject.SetActive(false);
            
        }
        else if (_player.CurrentHealth - _player.HeldSkill.Cost <= 0)
        {
            _costTweenMove = Tween
                .LocalPosition(skillRing.gameObject.transform, skillRing.rectTransform.localPosition,
                    new Vector3(0f, costY - 15f, 0f), 1.25f, Ease.OutCirc)
                .OnComplete(() => skillRing.gameObject.SetActive(false));
        }
        else
        {
            skillRing.gameObject.SetActive(true);
            _costTweenMove = Tween.Custom(
                skillRing.rectTransform.localPosition,
                skillRing.rectTransform.localPosition = new Vector3(0f, costY + 10f, 0f),
                0.5f,
                pos =>
                {
                    skillRing.rectTransform.localPosition = pos;
                },
                Ease.InOutQuint
            );
        }
    }

    private void OnSkillChanged(SkillBase skill)
    {
        skillImage.sprite = skill.icon;
        CheckCost();
    }

    private void OnBandagesChanged(int amount)
    {
        bandagesText.SetText("x{0}", amount);
    }

    private void OnBandageUsed(bool toggle)
    {
        bandageChains.gameObject.SetActive(toggle);
    }
    
    private void RefreshWeaponsUI()
    {
        SetWeaponSlot(weaponSlot0, _inventory.GetItemAtOffset(0));
        SetWeaponSlot(weaponSlot1, _inventory.GetItemAtOffset(1));
        SetWeaponSlot(weaponSlot2, _inventory.GetItemAtOffset(2));
    }

    private void SetWeaponSlot(Image image, WeaponBase weapon)
    {
        if (weapon == null)
        {
            image.enabled = false;
            return;
        }
        
        image.enabled = true;
        image.sprite = weapon.Icon.sprite;
    }
}