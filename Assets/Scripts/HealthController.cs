using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public float health, dHealth, realHealth;
    public float scaleFactor;
    private bool healing, damaging, usedSyringe;
    private InventoryItem syringe;
    public Image damageOverlay;
    public ItemGrid inventory;

    void Start()
    {
        health = 100;
    }

    void Update()
    {
        if (healing)
        {
            if (health < 100 && dHealth > 0)
            {
                health += scaleFactor;
                dHealth -= scaleFactor;
                DecreaseAlpha();
            } else {
                health = realHealth;
                healing = false;
            }
        }
        if (damaging)
        {
            if (health > 0 && dHealth < 0)
            {
                health -= scaleFactor;
                dHealth += scaleFactor;
                IncreaseAlpha();
            } else {
                health = realHealth;
                damaging = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ChangeHealth(-10);
        } else if (Input.GetKeyDown(KeyCode.H)) {
            foreach (InventoryItem item in inventory.actualItems) {
                if (item.itemData.itemName == "Health Syringe" && this.health < 100f) {
                    ChangeHealth(20);
                    usedSyringe = true;
                    syringe = item;
                    break;
                }
            }
            if (usedSyringe) {
                syringe.UseItem();
                usedSyringe = false;
            }
        }
    }

    private void IncreaseAlpha()
    {
        float dAlpha = (150f/255f) / (100f * (1/scaleFactor));
        if (damageOverlay.color.a + dAlpha < 200f/255f) {
            damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, damageOverlay.color.a + dAlpha);
        } else {
            damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, 150f/255f);
        }
    }

    private void DecreaseAlpha()
    {
        float dAlpha = (150f/255f) / (100f * (1/scaleFactor));
        if (damageOverlay.color.a + dAlpha > 0) {
            damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, damageOverlay.color.a - dAlpha);
        } else {
            damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, 0f);
        }
    }

    public void ChangeHealth(int health)
    {
        this.dHealth = health;
        if (this.dHealth > 0)
        {
            if (this.health + this.dHealth > 100f)
            {
                this.realHealth = 100f;
            } else {
                this.realHealth = this.health + health;
            }
            healing = true;
        } else {
            if (this.health + this.dHealth < 0)
            {
                this.realHealth = 0;
            } else {
                this.realHealth = this.health + health;
            }
            damaging = true;
        }
    }
}
