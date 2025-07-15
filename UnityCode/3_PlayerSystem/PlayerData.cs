using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Football Game/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Basic Info")]
    public string playerName;
    public int age;
    public string nationality;
    public int teamId;
    public PlayerPosition preferredPosition;
    public Sprite playerPhoto;
    
    [Header("Physical Attributes")]
    [Range(1, 99)]
    public int speed = 70;
    
    [Range(1, 99)]
    public int acceleration = 70;
    
    [Range(1, 99)]
    public int stamina = 70;
    
    [Range(1, 99)]
    public int strength = 70;
    
    [Range(1, 99)]
    public int jumping = 70;
    
    [Range(1, 99)]
    public int agility = 70;
    
    [Header("Technical Skills")]
    [Range(1, 99)]
    public int ballControl = 70;
    
    [Range(1, 99)]
    public int dribbling = 70;
    
    [Range(1, 99)]
    public int passing = 70;
    
    [Range(1, 99)]
    public int shooting = 70;
    
    [Range(1, 99)]
    public int technique = 70;
    
    [Range(1, 99)]
    public int finishing = 70;
    
    [Header("Mental Attributes")]
    [Range(1, 99)]
    public int vision = 70;
    
    [Range(1, 99)]
    public int positioning = 70;
    
    [Range(1, 99)]
    public int marking = 70;
    
    [Range(1, 99)]
    public int tackling = 70;
    
    [Range(1, 99)]
    public int aggression = 70;
    
    [Range(1, 99)]
    public int workRate = 70;
    
    [Header("Goalkeeping (if applicable)")]
    [Range(1, 99)]
    public int diving = 1;
    
    [Range(1, 99)]
    public int handling = 1;
    
    [Range(1, 99)]
    public int kicking = 1;
    
    [Range(1, 99)]
    public int positioning_gk = 1;
    
    [Range(1, 99)]
    public int reflexes = 1;
    
    [Header("Special Abilities")]
    public bool isSkillMoveSpecialist = false;
    public bool isSpeedDemon = false;
    public bool isPowerShooter = false;
    public bool isPlaymaker = false;
    public bool isDefensiveWall = false;
    public bool isGoalScorer = false;
    
    [Header("Weak Foot & Skill Moves")]
    [Range(1, 5)]
    public int weakFoot = 3;
    
    [Range(1, 5)]
    public int skillMoves = 3;
    
    [Header("Value")]
    public int marketValue = 1000000;
    public int wage = 10000;
    
    // Propiedad calculada para rating general
    public int overall
    {
        get
        {
            if (preferredPosition == PlayerPosition.Goalkeeper)
            {
                return CalculateGoalkeeperOverall();
            }
            else
            {
                return CalculateOutfieldOverall();
            }
        }
    }
    
    int CalculateOutfieldOverall()
    {
        int total = 0;
        int count = 0;
        
        // Atributos físicos (peso 25%)
        total += (speed + acceleration + stamina + strength + jumping + agility) * 25;
        count += 6 * 25;
        
        // Habilidades técnicas (peso 40%)
        total += (ballControl + dribbling + passing + shooting + technique + finishing) * 40;
        count += 6 * 40;
        
        // Atributos mentales (peso 35%)
        total += (vision + positioning + marking + tackling + aggression + workRate) * 35;
        count += 6 * 35;
        
        return total / count;
    }
    
    int CalculateGoalkeeperOverall()
    {
        int total = 0;
        int count = 0;
        
        // Atributos de portero (peso 60%)
        total += (diving + handling + kicking + positioning_gk + reflexes) * 60;
        count += 5 * 60;
        
        // Atributos físicos relevantes (peso 25%)
        total += (speed + acceleration + stamina + strength + jumping + agility) * 25;
        count += 6 * 25;
        
        // Atributos mentales (peso 15%)
        total += (vision + positioning + aggression + workRate) * 15;
        count += 4 * 15;
        
        return total / count;
    }
    
    // Método para obtener el rating en una posición específica
    public int GetPositionRating(PlayerPosition position)
    {
        switch (position)
        {
            case PlayerPosition.Goalkeeper:
                return CalculateGoalkeeperOverall();
                
            case PlayerPosition.CenterBack:
                return CalculateDefenderRating();
                
            case PlayerPosition.LeftBack:
            case PlayerPosition.RightBack:
                return CalculateFullBackRating();
                
            case PlayerPosition.DefensiveMidfield:
                return CalculateDMRating();
                
            case PlayerPosition.CentralMidfield:
                return CalculateCMRating();
                
            case PlayerPosition.AttackingMidfield:
                return CalculateAMRating();
                
            case PlayerPosition.LeftWing:
            case PlayerPosition.RightWing:
                return CalculateWingerRating();
                
            case PlayerPosition.Striker:
                return CalculateStrikerRating();
                
            default:
                return overall;
        }
    }
    
    int CalculateDefenderRating()
    {
        return (marking * 3 + tackling * 3 + positioning * 2 + strength * 2 + jumping * 2 + 
                speed + acceleration + stamina + agility + workRate + aggression) / 18;
    }
    
    int CalculateFullBackRating()
    {
        return (speed * 2 + acceleration * 2 + stamina * 2 + marking * 2 + tackling * 2 + 
                passing * 2 + positioning + strength + agility + workRate + aggression) / 17;
    }
    
    int CalculateDMRating()
    {
        return (tackling * 2 + marking * 2 + passing * 2 + positioning * 2 + workRate * 2 + 
                strength + stamina + ballControl + vision + aggression) / 16;
    }
    
    int CalculateCMRating()
    {
        return (passing * 3 + ballControl * 2 + vision * 2 + positioning * 2 + stamina * 2 + 
                technique + dribbling + marking + tackling + workRate) / 18;
    }
    
    int CalculateAMRating()
    {
        return (passing * 2 + ballControl * 2 + vision * 2 + technique * 2 + dribbling * 2 + 
                shooting + finishing + positioning + speed + agility) / 16;
    }
    
    int CalculateWingerRating()
    {
        return (speed * 3 + acceleration * 2 + dribbling * 2 + ballControl * 2 + agility * 2 + 
                passing + shooting + technique + stamina + workRate) / 17;
    }
    
    int CalculateStrikerRating()
    {
        return (shooting * 3 + finishing * 3 + positioning * 2 + ballControl * 2 + 
                strength + jumping + speed + acceleration + technique + agility) / 16;
    }
    
    // Método para generar un jugador aleatorio
    public static PlayerData GenerateRandomPlayer(string name, int teamId, PlayerPosition position)
    {
        PlayerData player = ScriptableObject.CreateInstance<PlayerData>();
        
        player.playerName = name;
        player.age = Random.Range(18, 35);
        player.teamId = teamId;
        player.preferredPosition = position;
        
        // Generar stats base aleatoriamente
        int baseRating = Random.Range(45, 85);
        int variation = Random.Range(-10, 10);
        
        player.speed = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        player.acceleration = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        player.stamina = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        player.strength = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        player.jumping = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        player.agility = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        
        player.ballControl = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        player.dribbling = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        player.passing = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        player.shooting = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        player.technique = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        player.finishing = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        
        player.vision = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        player.positioning = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        player.marking = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        player.tackling = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        player.aggression = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        player.workRate = Mathf.Clamp(baseRating + Random.Range(-variation, variation), 1, 99);
        
        // Ajustar stats según posición
        AdjustStatsForPosition(player, position);
        
        player.weakFoot = Random.Range(1, 6);
        player.skillMoves = Random.Range(1, 6);
        
        player.marketValue = baseRating * 50000;
        player.wage = baseRating * 500;
        
        return player;
    }
    
    static void AdjustStatsForPosition(PlayerData player, PlayerPosition position)
    {
        switch (position)
        {
            case PlayerPosition.Goalkeeper:
                player.diving = Random.Range(60, 95);
                player.handling = Random.Range(60, 95);
                player.kicking = Random.Range(50, 85);
                player.positioning_gk = Random.Range(60, 90);
                player.reflexes = Random.Range(60, 95);
                break;
                
            case PlayerPosition.CenterBack:
                player.marking += 10;
                player.tackling += 10;
                player.strength += 8;
                player.jumping += 8;
                player.speed -= 5;
                player.dribbling -= 8;
                break;
                
            case PlayerPosition.LeftBack:
            case PlayerPosition.RightBack:
                player.speed += 8;
                player.acceleration += 8;
                player.stamina += 10;
                player.shooting -= 10;
                player.finishing -= 10;
                break;
                
            case PlayerPosition.DefensiveMidfield:
                player.tackling += 8;
                player.marking += 8;
                player.passing += 5;
                player.shooting -= 8;
                player.speed -= 3;
                break;
                
            case PlayerPosition.CentralMidfield:
                player.passing += 10;
                player.ballControl += 8;
                player.vision += 8;
                player.stamina += 8;
                break;
                
            case PlayerPosition.AttackingMidfield:
                player.passing += 8;
                player.ballControl += 10;
                player.vision += 10;
                player.technique += 8;
                player.tackling -= 8;
                break;
                
            case PlayerPosition.LeftWing:
            case PlayerPosition.RightWing:
                player.speed += 10;
                player.acceleration += 10;
                player.dribbling += 10;
                player.agility += 8;
                player.strength -= 5;
                player.marking -= 10;
                break;
                
            case PlayerPosition.Striker:
                player.shooting += 10;
                player.finishing += 10;
                player.positioning += 8;
                player.strength += 5;
                player.marking -= 10;
                player.tackling -= 10;
                break;
        }
    }
}

public enum PlayerPosition
{
    Goalkeeper,
    CenterBack,
    LeftBack,
    RightBack,
    DefensiveMidfield,
    CentralMidfield,
    AttackingMidfield,
    LeftWing,
    RightWing,
    Striker
}