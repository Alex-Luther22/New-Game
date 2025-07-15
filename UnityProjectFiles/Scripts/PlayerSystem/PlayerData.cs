using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Datos completos del jugador con estadísticas realistas
/// Sistema de ratings estilo FIFA sin usar marcas registradas
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "New Player Data", menuName = "Football Game/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("📋 Información Básica")]
    public string playerName = "Player";
    public int playerNumber = 1;
    public int age = 25;
    public float height = 1.75f; // metros
    public float weight = 70f; // kg
    public PlayerPosition preferredPosition = PlayerPosition.CentralMidfield;
    public PlayerPosition[] alternativePositions;
    public string nationality = "Unknown";
    public string clubName = "Free Agent";
    
    [Header("⭐ Estadísticas Generales")]
    [Range(40, 99)]
    public int overall = 75;
    [Range(40, 99)]
    public int potential = 80;
    
    [Header("🏃 Atributos Físicos")]
    [Range(20, 99)]
    public int pace = 70;
    [Range(20, 99)]
    public int acceleration = 70;
    [Range(20, 99)]
    public int sprintSpeed = 70;
    [Range(20, 99)]
    public int agility = 70;
    [Range(20, 99)]
    public int balance = 70;
    [Range(20, 99)]
    public int jumping = 70;
    [Range(20, 99)]
    public int stamina = 70;
    [Range(20, 99)]
    public int strength = 70;
    
    [Header("⚽ Atributos de Disparo")]
    [Range(20, 99)]
    public int shooting = 65;
    [Range(20, 99)]
    public int finishing = 65;
    [Range(20, 99)]
    public int shotPower = 65;
    [Range(20, 99)]
    public int longShots = 65;
    [Range(20, 99)]
    public int volleys = 65;
    [Range(20, 99)]
    public int penalties = 65;
    
    [Header("🎯 Atributos de Pase")]
    [Range(20, 99)]
    public int passing = 70;
    [Range(20, 99)]
    public int shortPassing = 70;
    [Range(20, 99)]
    public int longPassing = 70;
    [Range(20, 99)]
    public int vision = 70;
    [Range(20, 99)]
    public int crossing = 70;
    [Range(20, 99)]
    public int freeKickAccuracy = 60;
    [Range(20, 99)]
    public int curve = 65;
    
    [Header("🎮 Atributos de Dribbling")]
    [Range(20, 99)]
    public int dribbling = 70;
    [Range(20, 99)]
    public int ballControl = 70;
    [Range(20, 99)]
    public int reactions = 70;
    [Range(20, 99)]
    public int composure = 70;
    
    [Header("🛡️ Atributos Defensivos")]
    [Range(20, 99)]
    public int defending = 60;
    [Range(20, 99)]
    public int interceptions = 60;
    [Range(20, 99)]
    public int headingAccuracy = 60;
    [Range(20, 99)]
    public int marking = 60;
    [Range(20, 99)]
    public int standingTackle = 60;
    [Range(20, 99)]
    public int slidingTackle = 60;
    
    [Header("🥅 Atributos de Portero")]
    [Range(20, 99)]
    public int goalkeeping = 30;
    [Range(20, 99)]
    public int gkDiving = 30;
    [Range(20, 99)]
    public int gkHandling = 30;
    [Range(20, 99)]
    public int gkKicking = 30;
    [Range(20, 99)]
    public int gkPositioning = 30;
    [Range(20, 99)]
    public int gkReflexes = 30;
    
    [Header("🧠 Atributos Mentales")]
    [Range(20, 99)]
    public int aggression = 50;
    [Range(20, 99)]
    public int positioning = 70;
    [Range(20, 99)]
    public int workRate = 70;
    [Range(20, 99)]
    public int leadership = 50;
    [Range(20, 99)]
    public int teamwork = 70;
    
    [Header("🎯 Especialidades")]
    public List<PlayerSpecialty> specialties = new List<PlayerSpecialty>();
    
    [Header("🏆 Historial")]
    public PlayerCareerStats careerStats;
    
    [Header("💰 Información Financiera")]
    public long marketValue = 1000000;
    public int wage = 10000;
    public int contractLength = 3;
    
    [Header("🎨 Personalización")]
    public PlayerAppearance appearance;
    public PlayerPlayStyle playStyle;
    
    [Header("⚙️ Configuración Técnica")]
    public WeakFoot weakFoot = WeakFoot.ThreeStar;
    public SkillMoves skillMoves = SkillMoves.ThreeStar;
    public AttackingWorkRate attackingWorkRate = AttackingWorkRate.Medium;
    public DefensiveWorkRate defensiveWorkRate = DefensiveWorkRate.Medium;
    
    // Propiedades calculadas
    public int Physical => Mathf.RoundToInt((stamina + strength + agility + balance + jumping + pace) / 6f);
    public int Mental => Mathf.RoundToInt((positioning + vision + composure + reactions + workRate) / 5f);
    public int Technical => Mathf.RoundToInt((ballControl + dribbling + shortPassing + longPassing + finishing) / 5f);
    
    void OnValidate()
    {
        // Calcular overall automáticamente
        CalculateOverall();
        
        // Asegurar que los valores estén en rango
        ClampValues();
        
        // Actualizar estadísticas específicas de posición
        UpdatePositionSpecificStats();
    }
    
    void CalculateOverall()
    {
        switch (preferredPosition)
        {
            case PlayerPosition.Goalkeeper:
                overall = CalculateGoalkeeperOverall();
                break;
            case PlayerPosition.CenterBack:
            case PlayerPosition.LeftBack:
            case PlayerPosition.RightBack:
                overall = CalculateDefenderOverall();
                break;
            case PlayerPosition.DefensiveMidfield:
            case PlayerPosition.CentralMidfield:
            case PlayerPosition.AttackingMidfield:
                overall = CalculateMidfielderOverall();
                break;
            case PlayerPosition.LeftWing:
            case PlayerPosition.RightWing:
            case PlayerPosition.Striker:
                overall = CalculateAttackerOverall();
                break;
        }
    }
    
    int CalculateGoalkeeperOverall()
    {
        return Mathf.RoundToInt((
            gkDiving * 0.21f +
            gkHandling * 0.21f +
            gkKicking * 0.15f +
            gkPositioning * 0.21f +
            gkReflexes * 0.21f +
            reactions * 0.05f +
            composure * 0.05f +
            strength * 0.05f +
            height * 10f // Bonus por altura
        ) / 1.05f);
    }
    
    int CalculateDefenderOverall()
    {
        return Mathf.RoundToInt((
            defending * 0.25f +
            headingAccuracy * 0.15f +
            marking * 0.15f +
            standingTackle * 0.15f +
            slidingTackle * 0.1f +
            strength * 0.1f +
            jumping * 0.1f +
            positioning * 0.1f +
            shortPassing * 0.1f +
            ballControl * 0.05f +
            reactions * 0.05f +
            composure * 0.05f +
            interceptions * 0.15f
        ) / 1.5f);
    }
    
    int CalculateMidfielderOverall()
    {
        return Mathf.RoundToInt((
            shortPassing * 0.2f +
            longPassing * 0.15f +
            ballControl * 0.15f +
            dribbling * 0.1f +
            vision * 0.15f +
            reactions * 0.1f +
            positioning * 0.1f +
            stamina * 0.1f +
            marking * 0.05f +
            interceptions * 0.05f +
            finishing * 0.05f +
            longShots * 0.05f
        ) / 1.25f);
    }
    
    int CalculateAttackerOverall()
    {
        return Mathf.RoundToInt((
            finishing * 0.2f +
            shotPower * 0.15f +
            dribbling * 0.15f +
            ballControl * 0.15f +
            pace * 0.15f +
            acceleration * 0.1f +
            sprintSpeed * 0.1f +
            positioning * 0.15f +
            reactions * 0.1f +
            composure * 0.1f +
            agility * 0.1f +
            balance * 0.1f +
            volleys * 0.05f +
            crossing * 0.05f
        ) / 1.45f);
    }
    
    void ClampValues()
    {
        // Asegurar que todos los valores estén entre 1 y 99
        overall = Mathf.Clamp(overall, 40, 99);
        potential = Mathf.Clamp(potential, overall, 99);
        
        // Clamp de atributos físicos
        pace = Mathf.Clamp(pace, 20, 99);
        acceleration = Mathf.Clamp(acceleration, 20, 99);
        sprintSpeed = Mathf.Clamp(sprintSpeed, 20, 99);
        agility = Mathf.Clamp(agility, 20, 99);
        balance = Mathf.Clamp(balance, 20, 99);
        jumping = Mathf.Clamp(jumping, 20, 99);
        stamina = Mathf.Clamp(stamina, 20, 99);
        strength = Mathf.Clamp(strength, 20, 99);
        
        // Clamp de atributos de disparo
        shooting = Mathf.Clamp(shooting, 20, 99);
        finishing = Mathf.Clamp(finishing, 20, 99);
        shotPower = Mathf.Clamp(shotPower, 20, 99);
        longShots = Mathf.Clamp(longShots, 20, 99);
        volleys = Mathf.Clamp(volleys, 20, 99);
        penalties = Mathf.Clamp(penalties, 20, 99);
        
        // Clamp de atributos de pase
        passing = Mathf.Clamp(passing, 20, 99);
        shortPassing = Mathf.Clamp(shortPassing, 20, 99);
        longPassing = Mathf.Clamp(longPassing, 20, 99);
        vision = Mathf.Clamp(vision, 20, 99);
        crossing = Mathf.Clamp(crossing, 20, 99);
        freeKickAccuracy = Mathf.Clamp(freeKickAccuracy, 20, 99);
        curve = Mathf.Clamp(curve, 20, 99);
        
        // Clamp de atributos de dribbling
        dribbling = Mathf.Clamp(dribbling, 20, 99);
        ballControl = Mathf.Clamp(ballControl, 20, 99);
        reactions = Mathf.Clamp(reactions, 20, 99);
        composure = Mathf.Clamp(composure, 20, 99);
        
        // Clamp de atributos defensivos
        defending = Mathf.Clamp(defending, 20, 99);
        interceptions = Mathf.Clamp(interceptions, 20, 99);
        headingAccuracy = Mathf.Clamp(headingAccuracy, 20, 99);
        marking = Mathf.Clamp(marking, 20, 99);
        standingTackle = Mathf.Clamp(standingTackle, 20, 99);
        slidingTackle = Mathf.Clamp(slidingTackle, 20, 99);
        
        // Clamp de atributos mentales
        aggression = Mathf.Clamp(aggression, 20, 99);
        positioning = Mathf.Clamp(positioning, 20, 99);
        workRate = Mathf.Clamp(workRate, 20, 99);
        leadership = Mathf.Clamp(leadership, 20, 99);
        teamwork = Mathf.Clamp(teamwork, 20, 99);
    }
    
    void UpdatePositionSpecificStats()
    {
        // Ajustar estadísticas específicas según la posición
        switch (preferredPosition)
        {
            case PlayerPosition.Goalkeeper:
                // Los porteros tienen stats defensivas y de disparo bajas
                defending = Mathf.Min(defending, 40);
                shooting = Mathf.Min(shooting, 30);
                dribbling = Mathf.Min(dribbling, 40);
                break;
                
            case PlayerPosition.CenterBack:
                // Los defensas centrales tienen stats de portería bajas
                goalkeeping = Mathf.Min(goalkeeping, 30);
                gkDiving = Mathf.Min(gkDiving, 30);
                gkHandling = Mathf.Min(gkHandling, 30);
                gkKicking = Mathf.Min(gkKicking, 30);
                gkPositioning = Mathf.Min(gkPositioning, 30);
                gkReflexes = Mathf.Min(gkReflexes, 30);
                break;
                
            case PlayerPosition.Striker:
                // Los delanteros tienen stats defensivas bajas
                defending = Mathf.Min(defending, 50);
                marking = Mathf.Min(marking, 40);
                standingTackle = Mathf.Min(standingTackle, 40);
                slidingTackle = Mathf.Min(slidingTackle, 40);
                break;
        }
    }
    
    public float GetPositionSuitability(PlayerPosition position)
    {
        if (position == preferredPosition)
            return 1.0f;
        
        if (alternativePositions != null)
        {
            foreach (PlayerPosition altPos in alternativePositions)
            {
                if (altPos == position)
                    return 0.8f;
            }
        }
        
        // Calcular compatibilidad basada en atributos
        return CalculatePositionCompatibility(position);
    }
    
    float CalculatePositionCompatibility(PlayerPosition position)
    {
        switch (position)
        {
            case PlayerPosition.Goalkeeper:
                return (goalkeeping + gkDiving + gkHandling + gkPositioning + gkReflexes) / 500f;
                
            case PlayerPosition.CenterBack:
                return (defending + headingAccuracy + marking + strength + positioning) / 500f;
                
            case PlayerPosition.LeftBack:
            case PlayerPosition.RightBack:
                return (defending + pace + crossing + stamina + marking) / 500f;
                
            case PlayerPosition.DefensiveMidfield:
                return (defending + shortPassing + interceptions + marking + workRate) / 500f;
                
            case PlayerPosition.CentralMidfield:
                return (shortPassing + longPassing + ballControl + vision + stamina) / 500f;
                
            case PlayerPosition.AttackingMidfield:
                return (shortPassing + dribbling + ballControl + vision + finishing) / 500f;
                
            case PlayerPosition.LeftWing:
            case PlayerPosition.RightWing:
                return (pace + crossing + dribbling + ballControl + agility) / 500f;
                
            case PlayerPosition.Striker:
                return (finishing + shotPower + positioning + reactions + composure) / 500f;
                
            default:
                return 0.5f;
        }
    }
    
    public void GenerateRandomPlayer(PlayerPosition position, int minOverall = 60, int maxOverall = 85)
    {
        // Generar jugador aleatorio para la posición específica
        preferredPosition = position;
        overall = Random.Range(minOverall, maxOverall + 1);
        potential = Random.Range(overall, 99);
        
        // Generar atributos base
        GenerateBaseAttributes();
        
        // Ajustar atributos según la posición
        AdjustAttributesForPosition(position);
        
        // Generar información básica
        GenerateBasicInfo();
        
        // Generar apariencia
        GenerateAppearance();
        
        // Validar valores
        OnValidate();
    }
    
    void GenerateBaseAttributes()
    {
        // Generar atributos con variación basada en el overall
        int baseVariation = 15;
        
        // Atributos físicos
        pace = RandomAttributeValue(baseVariation);
        acceleration = RandomAttributeValue(baseVariation);
        sprintSpeed = RandomAttributeValue(baseVariation);
        agility = RandomAttributeValue(baseVariation);
        balance = RandomAttributeValue(baseVariation);
        jumping = RandomAttributeValue(baseVariation);
        stamina = RandomAttributeValue(baseVariation);
        strength = RandomAttributeValue(baseVariation);
        
        // Atributos técnicos
        ballControl = RandomAttributeValue(baseVariation);
        dribbling = RandomAttributeValue(baseVariation);
        shortPassing = RandomAttributeValue(baseVariation);
        longPassing = RandomAttributeValue(baseVariation);
        finishing = RandomAttributeValue(baseVariation);
        shotPower = RandomAttributeValue(baseVariation);
        
        // Atributos mentales
        reactions = RandomAttributeValue(baseVariation);
        composure = RandomAttributeValue(baseVariation);
        vision = RandomAttributeValue(baseVariation);
        positioning = RandomAttributeValue(baseVariation);
        
        // Atributos defensivos
        defending = RandomAttributeValue(baseVariation);
        marking = RandomAttributeValue(baseVariation);
        standingTackle = RandomAttributeValue(baseVariation);
        slidingTackle = RandomAttributeValue(baseVariation);
        interceptions = RandomAttributeValue(baseVariation);
        headingAccuracy = RandomAttributeValue(baseVariation);
    }
    
    void AdjustAttributesForPosition(PlayerPosition position)
    {
        switch (position)
        {
            case PlayerPosition.Goalkeeper:
                // Boost de atributos de portería
                gkDiving = RandomAttributeValue(10, 20);
                gkHandling = RandomAttributeValue(10, 20);
                gkKicking = RandomAttributeValue(10, 20);
                gkPositioning = RandomAttributeValue(10, 20);
                gkReflexes = RandomAttributeValue(10, 20);
                
                // Reducir otros atributos
                pace = Mathf.Max(pace - 20, 30);
                dribbling = Mathf.Max(dribbling - 30, 20);
                finishing = Mathf.Max(finishing - 40, 20);
                break;
                
            case PlayerPosition.CenterBack:
                // Boost defensivo
                defending += 15;
                headingAccuracy += 15;
                marking += 15;
                strength += 10;
                
                // Reducir atributos ofensivos
                pace = Mathf.Max(pace - 10, 40);
                dribbling = Mathf.Max(dribbling - 10, 30);
                finishing = Mathf.Max(finishing - 20, 20);
                break;
                
            case PlayerPosition.Striker:
                // Boost ofensivo
                finishing += 15;
                shotPower += 15;
                positioning += 15;
                pace += 10;
                
                // Reducir atributos defensivos
                defending = Mathf.Max(defending - 20, 20);
                marking = Mathf.Max(marking - 20, 20);
                standingTackle = Mathf.Max(standingTackle - 20, 20);
                break;
                
            case PlayerPosition.CentralMidfield:
                // Boost de pase y control
                shortPassing += 15;
                longPassing += 10;
                ballControl += 10;
                vision += 10;
                stamina += 10;
                break;
                
            case PlayerPosition.LeftWing:
            case PlayerPosition.RightWing:
                // Boost de velocidad y cruce
                pace += 15;
                crossing += 15;
                dribbling += 10;
                agility += 10;
                break;
        }
    }
    
    void GenerateBasicInfo()
    {
        // Generar información básica aleatoria
        playerNumber = Random.Range(1, 100);
        age = Random.Range(18, 35);
        height = Random.Range(1.65f, 2.05f);
        weight = Random.Range(60f, 95f);
        
        // Generar nombre ficticio
        string[] firstNames = { "Alex", "Jordan", "Casey", "Morgan", "Riley", "Taylor", "Blake", "Cameron", "Quinn", "Avery" };
        string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };
        
        playerName = firstNames[Random.Range(0, firstNames.Length)] + " " + lastNames[Random.Range(0, lastNames.Length)];
        
        // Generar nacionalidad ficticia
        string[] nationalities = { "Footballia", "Soccerland", "Kickstan", "Goalopia", "Strikervania", "Passington", "Defendia", "Midfieldia" };
        nationality = nationalities[Random.Range(0, nationalities.Length)];
        
        // Generar club ficticio
        string[] clubNames = { "FC United", "Athletic Club", "Sports Club", "City FC", "United FC", "Rovers", "Wanderers", "Rangers" };
        clubName = clubNames[Random.Range(0, clubNames.Length)];
        
        // Generar valor de mercado basado en overall
        marketValue = (overall * overall * 1000) + Random.Range(-500000, 500000);
        wage = overall * 1000 + Random.Range(-5000, 5000);
        contractLength = Random.Range(1, 6);
    }
    
    void GenerateAppearance()
    {
        if (appearance == null)
        {
            appearance = new PlayerAppearance();
        }
        
        // Generar apariencia aleatoria
        appearance.skinColor = Random.Range(0, 6);
        appearance.hairColor = Random.Range(0, 8);
        appearance.hairStyle = Random.Range(0, 10);
        appearance.facialHair = Random.Range(0, 5);
        appearance.eyeColor = Random.Range(0, 5);
        appearance.bodyType = Random.Range(0, 3);
    }
    
    int RandomAttributeValue(int variation, int bonus = 0)
    {
        int baseValue = overall + bonus;
        int randomValue = baseValue + Random.Range(-variation, variation + 1);
        return Mathf.Clamp(randomValue, 20, 99);
    }
    
    public PlayerData CreateCopy()
    {
        PlayerData copy = CreateInstance<PlayerData>();
        
        // Copiar todos los valores
        copy.playerName = playerName;
        copy.playerNumber = playerNumber;
        copy.age = age;
        copy.height = height;
        copy.weight = weight;
        copy.preferredPosition = preferredPosition;
        copy.alternativePositions = alternativePositions;
        copy.nationality = nationality;
        copy.clubName = clubName;
        
        // Copiar atributos
        copy.overall = overall;
        copy.potential = potential;
        copy.pace = pace;
        copy.shooting = shooting;
        copy.passing = passing;
        copy.dribbling = dribbling;
        copy.defending = defending;
        copy.goalkeeping = goalkeeping;
        
        return copy;
    }
}

[System.Serializable]
public class PlayerCareerStats
{
    public int gamesPlayed;
    public int goals;
    public int assists;
    public int yellowCards;
    public int redCards;
    public int cleanSheets;
    public float averageRating;
    public int manOfTheMatchAwards;
}

[System.Serializable]
public class PlayerAppearance
{
    public int skinColor;
    public int hairColor;
    public int hairStyle;
    public int facialHair;
    public int eyeColor;
    public int bodyType;
}

[System.Serializable]
public enum PlayerSpecialty
{
    SpeedDemon,
    TechnicalDribbler,
    DeadBallSpecialist,
    AerialThreat,
    ClinicalFinisher,
    Playmaker,
    Defender,
    LeadershipQualities,
    SetPieceSpecialist,
    CounterAttackSpecialist
}

[System.Serializable]
public enum PlayerPlayStyle
{
    Balanced,
    Aggressive,
    Technical,
    Physical,
    Passer,
    Shooter,
    Defender,
    Speedster
}

[System.Serializable]
public enum WeakFoot
{
    OneStar = 1,
    TwoStar = 2,
    ThreeStar = 3,
    FourStar = 4,
    FiveStar = 5
}

[System.Serializable]
public enum SkillMoves
{
    OneStar = 1,
    TwoStar = 2,
    ThreeStar = 3,
    FourStar = 4,
    FiveStar = 5
}

[System.Serializable]
public enum AttackingWorkRate
{
    Low,
    Medium,
    High
}

[System.Serializable]
public enum DefensiveWorkRate
{
    Low,
    Medium,
    High
}