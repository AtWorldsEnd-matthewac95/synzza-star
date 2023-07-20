using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    /*
     * This script is configured to execute before the default time. In the Unity editor, go to Edit->Project Settings->Script Execution Order for more details.
     */
    // TODO - Rename to BattlerNavAgentMonocomponent
    public class BattlerMonocomponent : NavmeshAgentMonocomponent {
        [SerializeField] protected BattlerScrib _battler;
        [SerializeField] protected BattlerFactionScrib _faction;
        [SerializeField] protected BattlerStatusUI _statusText;
        [SerializeField] protected SkillScrib _demoAttackSkill;

        public UnityBattlerWorldObject WorldObject { get; protected set; }

        protected override void Awake() {
            base.Awake();

            WorldObject = new(_battler.ToBattler(), this);
            WorldObject.Battler.SetFaction(_faction.ToBattlerFaction());
            WorldObject.Battler.Status.OnBlockStatusChanged += OnBattlerBlockStatusChanged;

            if (_statusText != null) {
                _statusText.BattlerWorldObject = WorldObject;
            }
        }

        protected override void Start() {
            base.Start();

            WorldObject.Battler.Status.OnStationaryStatusChanged += OnBattlerStationaryStatusChanged;
            WorldObject.Battler.SetTargetSkill(_demoAttackSkill.ID);
        }

        protected override void Update() {
            base.Update();

            var battler = WorldObject.Battler;

            if (battler.CurrentTargetBattler == null || battler.CurrentTargetSkill == null) {
                return;
            }

            var isOpportune = battler.CurrentMeleeRules != BattlerMeleeRules.OpportuneAttack || battler.CurrentTargetBattler.Battler.Status.IsVulnerable;

            if (isOpportune) {
                var currentSkill = battler.CurrentMeleeRules switch {
                    BattlerMeleeRules.AutoAttack => battler.CurrentTargetSkill,
                    BattlerMeleeRules.OpportuneAttack => battler.CurrentTargetSkill,
                    BattlerMeleeRules.AutoBlock => battler.CurrentBlockSkill,
                    BattlerMeleeRules.AutoCounter => battler.CurrentBlockSkill,
                    _ => null
                };
                // TODO - change CurrentTargetBattler of AutoBlock and AutoCounter to CurrentBlockTargetBattler
                if (currentSkill?.Effect.IsEffectActivatible(WorldObject, battler.CurrentTargetBattler) ?? false) {
                    WorldObject.StartCoroutine(currentSkill.CreateCoroutine(WorldObject, battler.CurrentTargetBattler));
                }
            }
        }

        protected override void PickNewDestination() { }

        protected virtual void OnBattlerStationaryStatusChanged(bool isNowStationary) {
            if (_agent.isActiveAndEnabled) {
                _agent.isStopped = isNowStationary;

                if (_agent.isStopped) {
                    _agent.velocity = Vector3.zero;
                }
            }
        }

        protected virtual void OnCollisionEnter(Collision collision) {
            ReactToAttackHitbox(collision.collider);
        }

        protected virtual void OnTriggerEnter(Collider other) {
            ReactToAttackHitbox(other);
        }

        protected virtual void ReactToAttackHitbox(Collider collider) {
            if (WorldObject.IsInvincible || !collider.CompareTag("Attacks")) {
                return;
            }

            UnitySkillHitboxWorldObject hitbox = null;

            if (collider.gameObject.TryGetComponent(out SkillHitboxColliderMonocomponent colliderMono) && colliderMono.IsInitialized) {
                hitbox = colliderMono.Parent.WorldObject;
            } else if (collider.gameObject.TryGetComponent(out SkillHitboxMonocomponent hitboxMono) && hitboxMono.IsInitialized) {
                hitbox = hitboxMono.WorldObject;
            }

            if (hitbox != null) {
                WorldObject.ReactToSkillHitbox(hitbox);
                OnAttacked();
            }
        }

        protected virtual void OnAttacked() { }

        protected virtual void OnBattlerBlockStatusChanged(bool isBlockingNow) { }
    }
}
