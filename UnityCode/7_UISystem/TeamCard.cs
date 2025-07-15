using UnityEngine;
using UnityEngine.UI;

public class TeamCard : MonoBehaviour
{
    [Header("Team Info")]
    public Image teamLogo;
    public Text teamName;
    public Text teamRating;
    public Text teamCountry;
    
    [Header("Team Colors")]
    public Image primaryColorIndicator;
    public Image secondaryColorIndicator;
    
    [Header("Team Stats")]
    public Text attackStat;
    public Text midfieldStat;
    public Text defenseStat;
    public Text overallStat;
    
    [Header("UI Elements")]
    public Button selectButton;
    public Image cardBackground;
    public Image selectionHighlight;
    
    private TeamData teamData;
    private bool isSelected = false;
    
    public System.Action<TeamData> OnTeamSelected;
    
    void Start()
    {
        selectButton.onClick.AddListener(SelectTeam);
        selectionHighlight.gameObject.SetActive(false);
    }
    
    public void SetupTeam(TeamData team)
    {
        teamData = team;
        
        // Configurar información básica
        teamName.text = team.teamName;
        teamRating.text = team.overallRating.ToString();
        teamCountry.text = team.country;
        
        // Configurar logo (si existe)
        if (team.teamLogo != null)
        {
            teamLogo.sprite = team.teamLogo;
        }
        
        // Configurar colores
        primaryColorIndicator.color = team.primaryColor;
        secondaryColorIndicator.color = team.secondaryColor;
        
        // Configurar estadísticas
        attackStat.text = team.attackRating.ToString();
        midfieldStat.text = team.midfieldRating.ToString();
        defenseStat.text = team.defenseRating.ToString();
        overallStat.text = team.overallRating.ToString();
        
        // Configurar fondo basado en rating
        UpdateCardAppearance();
    }
    
    void UpdateCardAppearance()
    {
        if (teamData != null)
        {
            // Cambiar color del fondo basado en rating
            if (teamData.overallRating >= 85)
            {
                cardBackground.color = new Color(1f, 0.8f, 0f, 0.3f); // Dorado
            }
            else if (teamData.overallRating >= 75)
            {
                cardBackground.color = new Color(0.8f, 0.8f, 0.8f, 0.3f); // Plata
            }
            else if (teamData.overallRating >= 65)
            {
                cardBackground.color = new Color(0.8f, 0.5f, 0.2f, 0.3f); // Bronce
            }
            else
            {
                cardBackground.color = new Color(0.5f, 0.5f, 0.5f, 0.3f); // Gris
            }
        }
    }
    
    void SelectTeam()
    {
        OnTeamSelected?.Invoke(teamData);
        SetSelected(true);
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        selectionHighlight.gameObject.SetActive(selected);
        
        if (selected)
        {
            // Efectos visuales de selección
            transform.localScale = Vector3.one * 1.05f;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }
    
    public TeamData GetTeamData()
    {
        return teamData;
    }
    
    public bool IsSelected()
    {
        return isSelected;
    }
}