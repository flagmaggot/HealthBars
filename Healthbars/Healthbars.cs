﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SGUI;
using Dungeonator;

namespace HealthbarsFix
{
    class HealthbarBehavior : MonoBehaviour
    {
        BraveBehaviour parent;
        SGroup bar;
        float lastHealth;

        private void OnDeath(Vector2 _)
        {
            bar.Remove();
        }

        private void OnHealthChanged(float current, float max)
        {
            new SGroup()
            {
                With =
                {
                    new DamageLabelModifier(Mathf.RoundToInt(2*(current-lastHealth)).ToString(), (current>lastHealth) ? Color.green : Color.red),
                    //new SFadeOutAnimation(1.3f),
                    new FloatAwayAnimation(1.3f, parent.sprite.WorldTopCenter, new Vector2(0, -50)),
                    //new SInGameModifier()
                }
            };

            lastHealth = current;
        }

        public void Start()
        {
            parent = GetComponent<BraveBehaviour>();

            bar = new SGroup()
            {
                Size = new Vector2(50, 10),
                With =
                {
                    new HealthbarFix(parent)
                },
                Border = 2f
            };
            parent.healthHaver.OnDeath += OnDeath;
            parent.healthHaver.OnHealthChanged += OnHealthChanged;

            lastHealth = parent.healthHaver.GetCurrentHealth();
        }

        public void Update()
        {
            foreach (AIActor t in StaticReferenceManager.AllEnemies)
            {
                if (t.GetComponent<HealthbarBehavior>() == null)
                    t.gameObject.AddComponent<HealthbarBehavior>();
            }
        }
    }

    public class Healthbars : ETGModule
    {
        private void OnNewEnemy(Component component)
        {
            if (component.GetComponent<HealthbarBehavior>() == null)
                component.gameObject.AddComponent<HealthbarBehavior>();
        }

        public override void Start()
        {
            ETGMod.Objects.AddHook<AIActor>(OnNewEnemy);
            ETGMod.Objects.AddHook<PlayerController>(OnNewEnemy);
        }

        public override void Init()
        {
        }


        public override void Exit()
        {
        }
    }
}

