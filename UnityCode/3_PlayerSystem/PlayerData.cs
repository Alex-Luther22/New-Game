using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Football/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Basic Info")]
    public string playerName;
    public int playerId;
    public int teamId;
    public PlayerPosition preferredPosition;
    public int age;
    public string nationality;
    
    [Header("Physical Stats")]
    [Range(1, 99)]
    public int speed = 50;
    [Range(1, 99)]
    public int acceleration = 50;
    [Range(1, 99)]
    public int stamina = 50;
    [Range(1, 99)]
    public int strength = 50;
    [Range(1, 99)]
    public int agility = 50;
    [Range(1, 99)]
    public int jumping = 50;
    
    [Header("Technical Stats")]
    [Range(1, 99)]
    public int passing = 50;
    [Range(1, 99)]
    public int shooting = 50;
    [Range(1, 99)]
    public int dribbling = 50;
    [Range(1, 99)]
    public int ballControl = 50;
    [Range(1, 99)]
    public int crossing = 50;
    [Range(1, 99)]
    public int finishing = 50;
    [Range(1, 99)]
    public int technique = 50;
    
    [Header("Mental Stats")]
    [Range(1, 99)]
    public int vision = 50;
    [Range(1, 99)]
    public int positioning = 50;
    [Range(1, 99)]
    public int decision = 50;
    [Range(1, 99)]
    public int composure = 50;
    
    [Header("Defensive Stats")]
    [Range(1, 99)]
    public int tackling = 50;
    [Range(1, 99)]
    public int marking = 50;
    [Range(1, 99)]
    public int interception = 50;
    
    [Header("Goalkeeper Stats")]
    [Range(1, 99)]
    public int diving = 50;
    [Range(1, 99)]
    public int handling = 50;
    [Range(1, 99)]
    public int kicking = 50;
    [Range(1, 99)]
    public int reflexes = 50;
    [Range(1, 99)]
    public int positioning_gk = 50;
    
    [Header("Overall Rating")]
    [Range(1, 99)]
    public int overall = 50;
    
    [Header("Player Appearance")]
    public Material playerMaterial;
    public Sprite playerPhoto;
    public int shirtNumber;
    
    void OnValidate()
    {
        // Calculate overall rating automatically
        CalculateOverallRating();
    }
    
    public void CalculateOverallRating()
    {
        int totalStats = 0;
        int statCount = 0;
        
        // Physical stats
        totalStats += speed + acceleration + stamina + strength + agility + jumping;
        statCount += 6;
        
        // Technical stats
        totalStats += passing + shooting + dribbling + ballControl + crossing + finishing + technique;
        statCount += 7;
        
        // Mental stats
        totalStats += vision + positioning + decision + composure;
        statCount += 4;
        
        // Defensive stats (only for non-attackers)
        if (preferredPosition != PlayerPosition.Striker && preferredPosition != PlayerPosition.AttackingMidfield)
        {
            totalStats += tackling + marking + interception;
            statCount += 3;
        }
        
        // Goalkeeper stats (only for goalkeepers)
        if (preferredPosition == PlayerPosition.Goalkeeper)
        {
            totalStats += diving + handling + kicking + reflexes + positioning_gk;
            statCount += 5;
        }
        
        overall = Mathf.RoundToInt((float)totalStats / statCount);
        overall = Mathf.Clamp(overall, 1, 99);
    }
    
    public int GetStatByPosition()
    {
        switch (preferredPosition)
        {
            case PlayerPosition.Goalkeeper:
                return Mathf.RoundToInt((diving + handling + kicking + reflexes + positioning_gk) / 5f);
            case PlayerPosition.CenterBack:
            case PlayerPosition.LeftBack:
            case PlayerPosition.RightBack:
                return Mathf.RoundToInt((tackling + marking + interception + strength + positioning) / 5f);
            case PlayerPosition.DefensiveMidfield:
                return Mathf.RoundToInt((passing + tackling + interception + vision + positioning) / 5f);
            case PlayerPosition.CentralMidfield:
                return Mathf.RoundToInt((passing + vision + ballControl + technique + stamina) / 5f);
            case PlayerPosition.AttackingMidfield:
                return Mathf.RoundToInt((passing + vision + dribbling + technique + finishing) / 5f);
            case PlayerPosition.LeftWing:
            case PlayerPosition.RightWing:
                return Mathf.RoundToInt((speed + crossing + dribbling + agility + acceleration) / 5f);
            case PlayerPosition.Striker:
                return Mathf.RoundToInt((shooting + finishing + positioning + strength + technique) / 5f);
            default:
                return overall;
        }
    }
    
    public bool IsGoodAt(PlayerSkill skill)
    {
        switch (skill)
        {
            case PlayerSkill.Dribbling:
                return dribbling > 70;
            case PlayerSkill.Passing:
                return passing > 70;
            case PlayerSkill.Shooting:
                return shooting > 70;
            case PlayerSkill.Defending:
                return tackling > 70;
            case PlayerSkill.Speed:
                return speed > 70;
            default:
                return false;
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

public enum PlayerSkill
{
    Dribbling,
    Passing,
    Shooting,
    Defending,
    Speed
}

public enum TrickType
{
    StepOverRight,
    StepOverLeft,
    Nutmeg,
    Roulette,
    Elastico,
    RainbowFlick,
    HeelFlick,
    Scorpion,
    Rabona,
    Bicycle,
    Chop,
    CutInside,
    FakeShot,
    BodyFeint,
    Dummy,
    Spin
}