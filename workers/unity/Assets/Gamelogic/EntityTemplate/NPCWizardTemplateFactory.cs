using System;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Abilities;
using Improbable.Collections;
using Improbable.Core;
using Improbable.Fire;
using Improbable.Life;
using Improbable.Math;
using Improbable.Npc;
using Improbable.Team;
using Improbable.Unity.Core.Acls;
using Improbable.Worker;

namespace Assets.Gamelogic.EntityTemplate
{
    public static class NPCWizardTemplateFactory
    {
        public static SnapshotEntity CreateNPCWizardTemplate(Coordinates initialPosition, uint teamId)
        {
            int wizardType = new Random().Next(SimulationSettings.NPCWizardPrefabName.Length);

            var template = new SnapshotEntity {Prefab = SimulationSettings.NPCWizardPrefabName[wizardType] };
            template.Add(new TransformComponent.Data(initialPosition, 0));
            template.Add(new FSimAuthorityCheck.Data());
            template.Add(new Health.Data(SimulationSettings.WizardMaxHealth, SimulationSettings.WizardMaxHealth, true));
            template.Add(new Flammable.Data(false, true, FireEffectType.SMALL));
            template.Add(new TargetNavigation.Data(NavigationState.INACTIVE, Vector3f.ZERO, EntityId.InvalidEntityId, 0f));
            template.Add(new Spells.Data(new Map<SpellType, float> {{SpellType.LIGHTNING, 0f}, {SpellType.RAIN, 0f}},
                true));
            template.Add(new NPCWizard.Data(WizardFSMState.StateEnum.IDLE, EntityId.InvalidEntityId,
                SimulationSettings.InvalidPosition.ToVector3f()));
            template.Add(new TeamAssignment.Data(teamId));

            var permissions = Acl.Build()
                .SetReadAccess(CommonPredicates.PhysicsOrVisual)
                .SetWriteAccess<TransformComponent>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<FSimAuthorityCheck>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Health>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<TargetNavigation>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Spells>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<NPCWizard>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<TeamAssignment>(CommonPredicates.PhysicsOnly);

            template.SetAcl(permissions);

            return template;
        }
    }
}