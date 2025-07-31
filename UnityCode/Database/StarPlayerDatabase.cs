using System;
using System.Collections.Generic;
using UnityEngine;

namespace FootballMaster.PlayerSystem
{
    [Serializable]
    public class SpecialAbility
    {
        public string abilityId;
        public string abilityName;
        public string description;
        public AbilityType type;
        public int cooldownTime; // in seconds
        public float effectiveness; // 0.0 to 1.0
        public int energyCost;
        public bool isSignatureMove;
        public string animationTrigger;
        public AudioClip soundEffect;
    }

    [Serializable]
    public class StarPlayerTemplate
    {
        public string playerId;
        public string playerName;
        public string realWorldInspiration; // For documentation only
        public List<SpecialAbility> specialAbilities;
        public int overallRating;
        public Position position;
        public string nationality;
        public PlayStyle preferredPlayStyle;
        public List<TrickType> signatureTricks;
        public float freeKickAccuracy;
        public float penaltyAccuracy;
        public bool isLeftFooted;
    }

    public enum AbilityType
    {
        Dribbling,
        Shooting,
        Passing,
        Defending,
        Goalkeeping,
        Physical,
        Mental,
        Leadership,
        SetPiece
    }

    public enum PlayStyle
    {
        TechnicalDribbler,
        SpeedMerchant,
        PlayMaker,
        TargetMan,
        Poacher,
        Sweeper,
        BoxToBox,
        DeepLyingPlaymaker,
        WingBack,
        FalseNine,
        Regista,
        Libero
    }

    [CreateAssetMenu(fileName = "StarPlayerDatabase", menuName = "Football Master/Star Player Database")]
    public class StarPlayerDatabase : ScriptableObject
    {
        [Header("Star Player Templates")]
        public List<StarPlayerTemplate> starPlayers = new List<StarPlayerTemplate>();

        [Header("Special Abilities Database")]
        public List<SpecialAbility> availableAbilities = new List<SpecialAbility>();

        private void OnEnable()
        {
            if (starPlayers.Count == 0)
            {
                InitializeStarPlayers();
            }
            
            if (availableAbilities.Count == 0)
            {
                InitializeSpecialAbilities();
            }
        }

        private void InitializeSpecialAbilities()
        {
            // Dribbling Abilities
            availableAbilities.Add(new SpecialAbility
            {
                abilityId = "elastico_master",
                abilityName = "Elastico Master",
                description = "Performs devastating elastico moves that leave defenders behind",
                type = AbilityType.Dribbling,
                cooldownTime = 15,
                effectiveness = 0.85f,
                energyCost = 20,
                isSignatureMove = true,
                animationTrigger = "ElasticoMaster"
            });

            availableAbilities.Add(new SpecialAbility
            {
                abilityId = "rainbow_legend",
                abilityName = "Rainbow Legend",
                description = "Perfect rainbow flicks that mesmerize opponents",
                type = AbilityType.Dribbling,
                cooldownTime = 20,
                effectiveness = 0.90f,
                energyCost = 25,
                isSignatureMove = true,
                animationTrigger = "RainbowLegend"
            });

            availableAbilities.Add(new SpecialAbility
            {
                abilityId = "stepover_king",
                abilityName = "Stepover King",
                description = "Lightning-fast stepovers that create space instantly",
                type = AbilityType.Dribbling,
                cooldownTime = 10,
                effectiveness = 0.80f,
                energyCost = 15,
                isSignatureMove = false,
                animationTrigger = "StepoverKing"
            });

            // Shooting Abilities
            availableAbilities.Add(new SpecialAbility
            {
                abilityId = "rocket_shot",
                abilityName = "Rocket Shot",
                description = "Devastating power shots that can break through any defense",
                type = AbilityType.Shooting,
                cooldownTime = 25,
                effectiveness = 0.95f,
                energyCost = 30,
                isSignatureMove = true,
                animationTrigger = "RocketShot"
            });

            availableAbilities.Add(new SpecialAbility
            {
                abilityId = "curve_master",
                abilityName = "Curve Master",
                description = "Incredible curled shots that bend around goalkeepers",
                type = AbilityType.Shooting,
                cooldownTime = 20,
                effectiveness = 0.88f,
                energyCost = 25,
                isSignatureMove = true,
                animationTrigger = "CurveMaster"
            });

            availableAbilities.Add(new SpecialAbility
            {
                abilityId = "chip_specialist",
                abilityName = "Chip Specialist",
                description = "Perfect chip shots over advancing goalkeepers",
                type = AbilityType.Shooting,
                cooldownTime = 30,
                effectiveness = 0.85f,
                energyCost = 20,
                isSignatureMove = false,
                animationTrigger = "ChipSpecialist"
            });

            // Passing Abilities
            availableAbilities.Add(new SpecialAbility
            {
                abilityId = "vision_master",
                abilityName = "Vision Master",
                description = "Incredible through balls that split defenses",
                type = AbilityType.Passing,
                cooldownTime = 15,
                effectiveness = 0.92f,
                energyCost = 15,
                isSignatureMove = true,
                animationTrigger = "VisionMaster"
            });

            availableAbilities.Add(new SpecialAbility
            {
                abilityId = "long_pass_king",
                abilityName = "Long Pass King",
                description = "Pinpoint accurate long passes across the field",
                type = AbilityType.Passing,
                cooldownTime = 20,
                effectiveness = 0.90f,
                energyCost = 20,
                isSignatureMove = false,
                animationTrigger = "LongPassKing"
            });

            // Set Piece Abilities
            availableAbilities.Add(new SpecialAbility
            {
                abilityId = "free_kick_legend",
                abilityName = "Free Kick Legend",
                description = "Unstoppable free kicks with perfect curve and power",
                type = AbilityType.SetPiece,
                cooldownTime = 0, // Available when needed
                effectiveness = 0.95f,
                energyCost = 25,
                isSignatureMove = true,
                animationTrigger = "FreeKickLegend"
            });

            availableAbilities.Add(new SpecialAbility
            {
                abilityId = "penalty_master",
                abilityName = "Penalty Master",
                description = "Never misses penalties under pressure",
                type = AbilityType.SetPiece,
                cooldownTime = 0,
                effectiveness = 0.98f,
                energyCost = 10,
                isSignatureMove = false,
                animationTrigger = "PenaltyMaster"
            });

            // Physical Abilities
            availableAbilities.Add(new SpecialAbility
            {
                abilityId = "lightning_pace",
                abilityName = "Lightning Pace",
                description = "Explosive speed bursts that leave defenders behind",
                type = AbilityType.Physical,
                cooldownTime = 30,
                effectiveness = 0.90f,
                energyCost = 35,
                isSignatureMove = false,
                animationTrigger = "LightningPace"
            });

            availableAbilities.Add(new SpecialAbility
            {
                abilityId = "aerial_dominance",
                abilityName = "Aerial Dominance",
                description = "Incredible jumping ability and header accuracy",
                type = AbilityType.Physical,
                cooldownTime = 20,
                effectiveness = 0.88f,
                energyCost = 25,
                isSignatureMove = false,
                animationTrigger = "AerialDominance"
            });

            // Leadership Abilities
            availableAbilities.Add(new SpecialAbility
            {
                abilityId = "captain_influence",
                abilityName = "Captain's Influence",
                description = "Inspires teammates to perform better in crucial moments",
                type = AbilityType.Leadership,
                cooldownTime = 120,
                effectiveness = 0.75f,
                energyCost = 0,
                isSignatureMove = false,
                animationTrigger = "CaptainInfluence"
            });
        }

        private void InitializeStarPlayers()
        {
            // Kylian Mbappe inspired (Lightning Striker)
            starPlayers.Add(new StarPlayerTemplate
            {
                playerId = "lightning_striker_01",
                playerName = "Kylian Mbappe",
                realWorldInspiration = "Based on Kylian Mbappe's playstyle",
                overallRating = 91,
                position = Position.Forward,
                nationality = "France",
                preferredPlayStyle = PlayStyle.SpeedMerchant,
                signatureTricks = new List<TrickType> { TrickType.StepOverLeft, TrickType.FakeShot, TrickType.CutInside },
                freeKickAccuracy = 0.75f,
                penaltyAccuracy = 0.88f,
                isLeftFooted = false,
                specialAbilities = new List<SpecialAbility>
                {
                    GetAbilityById("lightning_pace"),
                    GetAbilityById("rocket_shot"),
                    GetAbilityById("stepover_king")
                }
            });

            // Vinicius Jr inspired (Brazilian Magician)
            starPlayers.Add(new StarPlayerTemplate
            {
                playerId = "brazilian_magician_01",
                playerName = "Vinicius Junior",
                realWorldInspiration = "Based on Vinicius Jr's playstyle",
                overallRating = 89,
                position = Position.Forward,
                nationality = "Brazil",
                preferredPlayStyle = PlayStyle.TechnicalDribbler,
                signatureTricks = new List<TrickType> { TrickType.Elastico, TrickType.RainbowFlick, TrickType.Roulette },
                freeKickAccuracy = 0.70f,
                penaltyAccuracy = 0.82f,
                isLeftFooted = false,
                specialAbilities = new List<SpecialAbility>
                {
                    GetAbilityById("rainbow_legend"),
                    GetAbilityById("elastico_master"),
                    GetAbilityById("curve_master")
                }
            });

            // Kevin De Bruyne inspired (Midfield Maestro)
            starPlayers.Add(new StarPlayerTemplate
            {
                playerId = "midfield_maestro_01",
                playerName = "Kevin De Bruyne",
                realWorldInspiration = "Based on Kevin De Bruyne's playstyle",
                overallRating = 91,
                position = Position.Midfielder,
                nationality = "Belgium",
                preferredPlayStyle = PlayStyle.PlayMaker,
                signatureTricks = new List<TrickType> { TrickType.BodyFeint, TrickType.FakeShot, TrickType.Dummy },
                freeKickAccuracy = 0.92f,
                penaltyAccuracy = 0.85f,
                isLeftFooted = false,
                specialAbilities = new List<SpecialAbility>
                {
                    GetAbilityById("vision_master"),
                    GetAbilityById("long_pass_king"),
                    GetAbilityById("free_kick_legend")
                }
            });

            // Erling Haaland inspired (Goal Machine)
            starPlayers.Add(new StarPlayerTemplate
            {
                playerId = "goal_machine_01",
                playerName = "Erling Haaland",
                realWorldInspiration = "Based on Erling Haaland's playstyle",
                overallRating = 91,
                position = Position.Forward,
                nationality = "Norway",
                preferredPlayStyle = PlayStyle.TargetMan,
                signatureTricks = new List<TrickType> { TrickType.FakeShot, TrickType.BodyFeint, TrickType.Chop },
                freeKickAccuracy = 0.65f,
                penaltyAccuracy = 0.90f,
                isLeftFooted = true,
                specialAbilities = new List<SpecialAbility>
                {
                    GetAbilityById("rocket_shot"),
                    GetAbilityById("aerial_dominance"),
                    GetAbilityById("penalty_master")
                }
            });

            // Jude Bellingham inspired (Young Phenomenon)
            starPlayers.Add(new StarPlayerTemplate
            {
                playerId = "young_phenomenon_01",
                playerName = "Jude Bellingham",
                realWorldInspiration = "Based on Jude Bellingham's playstyle",
                overallRating = 87,
                position = Position.Midfielder,
                nationality = "England",
                preferredPlayStyle = PlayStyle.BoxToBox,
                signatureTricks = new List<TrickType> { TrickType.Roulette, TrickType.StepOverRight, TrickType.Spin },
                freeKickAccuracy = 0.78f,
                penaltyAccuracy = 0.83f,
                isLeftFooted = false,
                specialAbilities = new List<SpecialAbility>
                {
                    GetAbilityById("captain_influence"),
                    GetAbilityById("vision_master"),
                    GetAbilityById("lightning_pace")
                }
            });

            // Add more star players...
            InitializeMoreStarPlayers();
        }

        private void InitializeMoreStarPlayers()
        {
            // Luka Modric inspired (Timeless Maestro)
            starPlayers.Add(new StarPlayerTemplate
            {
                playerId = "timeless_maestro_01",
                playerName = "Luka Modric",
                realWorldInspiration = "Based on Luka Modric's playstyle",
                overallRating = 88,
                position = Position.Midfielder,
                nationality = "Croatia",
                preferredPlayStyle = PlayStyle.DeepLyingPlaymaker,
                signatureTricks = new List<TrickType> { TrickType.Roulette, TrickType.BodyFeint, TrickType.Dummy },
                freeKickAccuracy = 0.85f,
                penaltyAccuracy = 0.80f,
                isLeftFooted = false,
                specialAbilities = new List<SpecialAbility>
                {
                    GetAbilityById("vision_master"),
                    GetAbilityById("captain_influence"),
                    GetAbilityById("curve_master")
                }
            });

            // Virgil van Dijk inspired (Defensive Colossus)
            starPlayers.Add(new StarPlayerTemplate
            {
                playerId = "defensive_colossus_01",
                playerName = "Virgil van Dijk",
                realWorldInspiration = "Based on Virgil van Dijk's playstyle",
                overallRating = 89,
                position = Position.Defender,
                nationality = "Netherlands",
                preferredPlayStyle = PlayStyle.Sweeper,
                signatureTricks = new List<TrickType> { TrickType.BodyFeint, TrickType.Dummy },
                freeKickAccuracy = 0.82f,
                penaltyAccuracy = 0.75f,
                isLeftFooted = true,
                specialAbilities = new List<SpecialAbility>
                {
                    GetAbilityById("aerial_dominance"),
                    GetAbilityById("captain_influence"),
                    GetAbilityById("long_pass_king")
                }
            });

            // Pedri inspired (Young Virtuoso)
            starPlayers.Add(new StarPlayerTemplate
            {
                playerId = "young_virtuoso_01",
                playerName = "Pedri Gonzalez",
                realWorldInspiration = "Based on Pedri's playstyle",
                overallRating = 85,
                position = Position.Midfielder,
                nationality = "Spain",
                preferredPlayStyle = PlayStyle.TechnicalDribbler,
                signatureTricks = new List<TrickType> { TrickType.Roulette, TrickType.Elastico, TrickType.BodyFeint },
                freeKickAccuracy = 0.75f,
                penaltyAccuracy = 0.78f,
                isLeftFooted = false,
                specialAbilities = new List<SpecialAbility>
                {
                    GetAbilityById("vision_master"),
                    GetAbilityById("elastico_master"),
                    GetAbilityById("curve_master")
                }
            });
        }

        private SpecialAbility GetAbilityById(string abilityId)
        {
            return availableAbilities.Find(ability => ability.abilityId == abilityId);
        }

        public StarPlayerTemplate GetStarPlayerById(string playerId)
        {
            return starPlayers.Find(player => player.playerId == playerId);
        }

        public List<StarPlayerTemplate> GetStarPlayersByPosition(Position position)
        {
            return starPlayers.FindAll(player => player.position == position);
        }

        public List<StarPlayerTemplate> GetStarPlayersByNationality(string nationality)
        {
            return starPlayers.FindAll(player => player.nationality == nationality);
        }

        public SpecialAbility GetRandomAbilityByType(AbilityType type)
        {
            var abilitiesOfType = availableAbilities.FindAll(ability => ability.type == type);
            if (abilitiesOfType.Count > 0)
            {
                return abilitiesOfType[UnityEngine.Random.Range(0, abilitiesOfType.Count)];
            }
            return null;
        }
    }
}