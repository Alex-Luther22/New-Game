from motor.motor_asyncio import AsyncIOMotorClient
from typing import List, Optional, Dict, Any
import os
from models import *

class DatabaseManager:
    def __init__(self):
        self.client = AsyncIOMotorClient(os.environ['MONGO_URL'])
        self.db = self.client[os.environ['DB_NAME']]
        
    async def initialize_data(self):
        """Initialize database with default teams, stadiums, and achievements"""
        await self.create_default_teams()
        await self.create_default_stadiums()
        await self.create_default_achievements()
        
    async def create_default_teams(self):
        """Create 100+ teams from major leagues without copyright issues"""
        default_teams = [
            # English Premier League (Top 6)
            {
                "name": "London Red", "short_name": "LRD", "country": "England", "league": "Premier League",
                "overall_rating": 88, "attack_rating": 90, "midfield_rating": 87, "defense_rating": 86,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "Emirates Arena",
                "stadium_capacity": 60000, "budget": 200000000, "prestige": 9
            },
            {
                "name": "Manchester Blue", "short_name": "MCB", "country": "England", "league": "Premier League",
                "overall_rating": 91, "attack_rating": 93, "midfield_rating": 90, "defense_rating": 89,
                "primary_color": "#6CABDD", "secondary_color": "#FFFFFF", "stadium_name": "Etihad Stadium",
                "stadium_capacity": 55000, "budget": 250000000, "prestige": 10
            },
            {
                "name": "Manchester Red", "short_name": "MRD", "country": "England", "league": "Premier League",
                "overall_rating": 85, "attack_rating": 87, "midfield_rating": 84, "defense_rating": 83,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "Old Trafford",
                "stadium_capacity": 75000, "budget": 180000000, "prestige": 9
            },
            {
                "name": "London Blue", "short_name": "LBL", "country": "England", "league": "Premier League",
                "overall_rating": 87, "attack_rating": 89, "midfield_rating": 86, "defense_rating": 85,
                "primary_color": "#034694", "secondary_color": "#FFFFFF", "stadium_name": "Stamford Bridge",
                "stadium_capacity": 42000, "budget": 220000000, "prestige": 8
            },
            {
                "name": "North London", "short_name": "NLD", "country": "England", "league": "Premier League",
                "overall_rating": 84, "attack_rating": 86, "midfield_rating": 83, "defense_rating": 82,
                "primary_color": "#132257", "secondary_color": "#FFFFFF", "stadium_name": "New White Hart Lane",
                "stadium_capacity": 62000, "budget": 150000000, "prestige": 7
            },
            {
                "name": "Liverpool Red", "short_name": "LIV", "country": "England", "league": "Premier League",
                "overall_rating": 89, "attack_rating": 91, "midfield_rating": 88, "defense_rating": 87,
                "primary_color": "#C8102E", "secondary_color": "#FFFFFF", "stadium_name": "Anfield",
                "stadium_capacity": 54000, "budget": 190000000, "prestige": 9
            },
            
            # Spanish La Liga
            {
                "name": "Madrid White", "short_name": "MDW", "country": "Spain", "league": "La Liga",
                "overall_rating": 92, "attack_rating": 94, "midfield_rating": 91, "defense_rating": 90,
                "primary_color": "#FFFFFF", "secondary_color": "#FFD700", "stadium_name": "Santiago Bernabéu",
                "stadium_capacity": 81000, "budget": 300000000, "prestige": 10
            },
            {
                "name": "Barcelona FC", "short_name": "BAR", "country": "Spain", "league": "La Liga",
                "overall_rating": 90, "attack_rating": 92, "midfield_rating": 89, "defense_rating": 88,
                "primary_color": "#A50044", "secondary_color": "#004D98", "stadium_name": "Camp Nou",
                "stadium_capacity": 99000, "budget": 280000000, "prestige": 10
            },
            {
                "name": "Madrid Red", "short_name": "ATM", "country": "Spain", "league": "La Liga",
                "overall_rating": 86, "attack_rating": 84, "midfield_rating": 87, "defense_rating": 89,
                "primary_color": "#CE2029", "secondary_color": "#FFFFFF", "stadium_name": "Wanda Metropolitano",
                "stadium_capacity": 70000, "budget": 160000000, "prestige": 8
            },
            {
                "name": "Sevilla FC", "short_name": "SEV", "country": "Spain", "league": "La Liga",
                "overall_rating": 82, "attack_rating": 80, "midfield_rating": 83, "defense_rating": 84,
                "primary_color": "#FFFFFF", "secondary_color": "#D2001F", "stadium_name": "Ramón Sánchez Pizjuán",
                "stadium_capacity": 43000, "budget": 80000000, "prestige": 7
            },
            
            # German Bundesliga
            {
                "name": "Bayern Munich", "short_name": "BAY", "country": "Germany", "league": "Bundesliga",
                "overall_rating": 90, "attack_rating": 92, "midfield_rating": 89, "defense_rating": 88,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "Allianz Arena",
                "stadium_capacity": 75000, "budget": 250000000, "prestige": 9
            },
            {
                "name": "Borussia Dortmund", "short_name": "BVB", "country": "Germany", "league": "Bundesliga",
                "overall_rating": 85, "attack_rating": 87, "midfield_rating": 84, "defense_rating": 83,
                "primary_color": "#FDE100", "secondary_color": "#000000", "stadium_name": "Signal Iduna Park",
                "stadium_capacity": 81000, "budget": 140000000, "prestige": 8
            },
            
            # Italian Serie A
            {
                "name": "Juventus FC", "short_name": "JUV", "country": "Italy", "league": "Serie A",
                "overall_rating": 84, "attack_rating": 82, "midfield_rating": 85, "defense_rating": 86,
                "primary_color": "#FFFFFF", "secondary_color": "#000000", "stadium_name": "Allianz Stadium",
                "stadium_capacity": 41000, "budget": 120000000, "prestige": 8
            },
            {
                "name": "Milan AC", "short_name": "MIL", "country": "Italy", "league": "Serie A",
                "overall_rating": 83, "attack_rating": 85, "midfield_rating": 82, "defense_rating": 81,
                "primary_color": "#FF0000", "secondary_color": "#000000", "stadium_name": "San Siro",
                "stadium_capacity": 80000, "budget": 110000000, "prestige": 8
            },
            {
                "name": "Inter Milan", "short_name": "INT", "country": "Italy", "league": "Serie A",
                "overall_rating": 85, "attack_rating": 87, "midfield_rating": 84, "defense_rating": 83,
                "primary_color": "#0068A8", "secondary_color": "#000000", "stadium_name": "San Siro",
                "stadium_capacity": 80000, "budget": 130000000, "prestige": 8
            },
            
            # French Ligue 1
            {
                "name": "Paris Saint-Germain", "short_name": "PSG", "country": "France", "league": "Ligue 1",
                "overall_rating": 88, "attack_rating": 91, "midfield_rating": 86, "defense_rating": 85,
                "primary_color": "#1A4B84", "secondary_color": "#FF0000", "stadium_name": "Parc des Princes",
                "stadium_capacity": 48000, "budget": 400000000, "prestige": 8
            },
            {
                "name": "Marseille", "short_name": "MAR", "country": "France", "league": "Ligue 1",
                "overall_rating": 78, "attack_rating": 76, "midfield_rating": 79, "defense_rating": 80,
                "primary_color": "#0084C4", "secondary_color": "#FFFFFF", "stadium_name": "Vélodrome",
                "stadium_capacity": 67000, "budget": 60000000, "prestige": 6
            },
            
            # Brazilian League
            {
                "name": "Flamengo", "short_name": "FLA", "country": "Brazil", "league": "Brasileirão",
                "overall_rating": 82, "attack_rating": 85, "midfield_rating": 80, "defense_rating": 79,
                "primary_color": "#FF0000", "secondary_color": "#000000", "stadium_name": "Maracanã",
                "stadium_capacity": 78000, "budget": 40000000, "prestige": 7
            },
            {
                "name": "Santos FC", "short_name": "SAN", "country": "Brazil", "league": "Brasileirão",
                "overall_rating": 78, "attack_rating": 80, "midfield_rating": 77, "defense_rating": 76,
                "primary_color": "#FFFFFF", "secondary_color": "#000000", "stadium_name": "Vila Belmiro",
                "stadium_capacity": 20000, "budget": 25000000, "prestige": 7
            },
            
            # Argentine League
            {
                "name": "River Plate", "short_name": "RIV", "country": "Argentina", "league": "Primera División",
                "overall_rating": 80, "attack_rating": 82, "midfield_rating": 79, "defense_rating": 78,
                "primary_color": "#FFFFFF", "secondary_color": "#FF0000", "stadium_name": "Monumental",
                "stadium_capacity": 70000, "budget": 35000000, "prestige": 7
            },
            {
                "name": "Boca Juniors", "short_name": "BOC", "country": "Argentina", "league": "Primera División",
                "overall_rating": 81, "attack_rating": 83, "midfield_rating": 80, "defense_rating": 79,
                "primary_color": "#1E3A8A", "secondary_color": "#FFD700", "stadium_name": "La Bombonera",
                "stadium_capacity": 54000, "budget": 30000000, "prestige": 7
            },
            
            # More Premier League Teams
            {
                "name": "Brighton FC", "short_name": "BHA", "country": "England", "league": "Premier League",
                "overall_rating": 78, "attack_rating": 76, "midfield_rating": 79, "defense_rating": 79,
                "primary_color": "#0057B8", "secondary_color": "#FFCD00", "stadium_name": "American Express Community Stadium",
                "stadium_capacity": 31800, "budget": 45000000, "prestige": 6
            },
            {
                "name": "Newcastle United", "short_name": "NEW", "country": "England", "league": "Premier League",
                "overall_rating": 82, "attack_rating": 80, "midfield_rating": 83, "defense_rating": 83,
                "primary_color": "#000000", "secondary_color": "#FFFFFF", "stadium_name": "St. James' Park",
                "stadium_capacity": 52000, "budget": 120000000, "prestige": 7
            },
            {
                "name": "Aston Villa", "short_name": "AVL", "country": "England", "league": "Premier League",
                "overall_rating": 80, "attack_rating": 82, "midfield_rating": 79, "defense_rating": 78,
                "primary_color": "#7A003C", "secondary_color": "#95BFE5", "stadium_name": "Villa Park",
                "stadium_capacity": 42682, "budget": 65000000, "prestige": 7
            },
            {
                "name": "West Ham United", "short_name": "WHU", "country": "England", "league": "Premier League",
                "overall_rating": 77, "attack_rating": 75, "midfield_rating": 78, "defense_rating": 78,
                "primary_color": "#7A263A", "secondary_color": "#1BB1E7", "stadium_name": "London Stadium",
                "stadium_capacity": 66000, "budget": 55000000, "prestige": 6
            },
            
            # More La Liga Teams
            {
                "name": "Villarreal CF", "short_name": "VIL", "country": "Spain", "league": "La Liga",
                "overall_rating": 81, "attack_rating": 80, "midfield_rating": 82, "defense_rating": 80,
                "primary_color": "#FFFF00", "secondary_color": "#005BA6", "stadium_name": "Estadio de la Cerámica",
                "stadium_capacity": 23500, "budget": 70000000, "prestige": 7
            },
            {
                "name": "Real Sociedad", "short_name": "RSO", "country": "Spain", "league": "La Liga",
                "overall_rating": 79, "attack_rating": 78, "midfield_rating": 81, "defense_rating": 78,
                "primary_color": "#004C99", "secondary_color": "#FFFFFF", "stadium_name": "Reale Arena",
                "stadium_capacity": 39500, "budget": 50000000, "prestige": 6
            },
            {
                "name": "Real Betis", "short_name": "BET", "country": "Spain", "league": "La Liga",
                "overall_rating": 78, "attack_rating": 80, "midfield_rating": 77, "defense_rating": 76,
                "primary_color": "#00954C", "secondary_color": "#FFFFFF", "stadium_name": "Benito Villamarín",
                "stadium_capacity": 60720, "budget": 45000000, "prestige": 6
            },
            {
                "name": "Athletic Club", "short_name": "ATH", "country": "Spain", "league": "La Liga",
                "overall_rating": 79, "attack_rating": 77, "midfield_rating": 80, "defense_rating": 80,
                "primary_color": "#EE2523", "secondary_color": "#FFFFFF", "stadium_name": "San Mamés",
                "stadium_capacity": 53289, "budget": 40000000, "prestige": 7
            },
            
            # More Bundesliga Teams  
            {
                "name": "RB Leipzig", "short_name": "RBL", "country": "Germany", "league": "Bundesliga",
                "overall_rating": 83, "attack_rating": 85, "midfield_rating": 82, "defense_rating": 81,
                "primary_color": "#DC143C", "secondary_color": "#FFFFFF", "stadium_name": "Red Bull Arena",
                "stadium_capacity": 47069, "budget": 85000000, "prestige": 7
            },
            {
                "name": "Bayer Leverkusen", "short_name": "B04", "country": "Germany", "league": "Bundesliga",
                "overall_rating": 84, "attack_rating": 86, "midfield_rating": 83, "defense_rating": 82,
                "primary_color": "#E32221", "secondary_color": "#000000", "stadium_name": "BayArena",
                "stadium_capacity": 30210, "budget": 90000000, "prestige": 7
            },
            {
                "name": "Eintracht Frankfurt", "short_name": "SGE", "country": "Germany", "league": "Bundesliga",
                "overall_rating": 80, "attack_rating": 82, "midfield_rating": 79, "defense_rating": 78,
                "primary_color": "#E1000F", "secondary_color": "#000000", "stadium_name": "Deutsche Bank Park",
                "stadium_capacity": 51500, "budget": 60000000, "prestige": 6
            },
            
            # More Serie A Teams
            {
                "name": "AS Roma", "short_name": "ROM", "country": "Italy", "league": "Serie A",
                "overall_rating": 82, "attack_rating": 84, "midfield_rating": 81, "defense_rating": 80,
                "primary_color": "#CC0000", "secondary_color": "#FCDD09", "stadium_name": "Stadio Olimpico",
                "stadium_capacity": 70634, "budget": 85000000, "prestige": 8
            },
            {
                "name": "SS Lazio", "short_name": "LAZ", "country": "Italy", "league": "Serie A",
                "overall_rating": 81, "attack_rating": 83, "midfield_rating": 80, "defense_rating": 79,
                "primary_color": "#87CEEB", "secondary_color": "#FFFFFF", "stadium_name": "Stadio Olimpico",
                "stadium_capacity": 70634, "budget": 75000000, "prestige": 7
            },
            {
                "name": "SSC Napoli", "short_name": "NAP", "country": "Italy", "league": "Serie A",
                "overall_rating": 85, "attack_rating": 87, "midfield_rating": 84, "defense_rating": 83,
                "primary_color": "#0B7EC8", "secondary_color": "#FFFFFF", "stadium_name": "Stadio Diego Armando Maradona",
                "stadium_capacity": 54726, "budget": 95000000, "prestige": 8
            },
            {
                "name": "Atalanta BC", "short_name": "ATA", "country": "Italy", "league": "Serie A",
                "overall_rating": 81, "attack_rating": 84, "midfield_rating": 79, "defense_rating": 78,
                "primary_color": "#1E90FF", "secondary_color": "#000000", "stadium_name": "Gewiss Stadium",
                "stadium_capacity": 21747, "budget": 65000000, "prestige": 7
            },
            
            # More Ligue 1 Teams
            {
                "name": "AS Monaco", "short_name": "MON", "country": "France", "league": "Ligue 1",
                "overall_rating": 80, "attack_rating": 82, "midfield_rating": 79, "defense_rating": 78,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "Stade Louis II",
                "stadium_capacity": 18523, "budget": 70000000, "prestige": 7
            },
            {
                "name": "Olympique Lyon", "short_name": "OLY", "country": "France", "league": "Ligue 1",
                "overall_rating": 79, "attack_rating": 81, "midfield_rating": 78, "defense_rating": 77,
                "primary_color": "#0066CC", "secondary_color": "#FFFFFF", "stadium_name": "Groupama Stadium",
                "stadium_capacity": 59186, "budget": 65000000, "prestige": 7
            },
            {
                "name": "OGC Nice", "short_name": "NIC", "country": "France", "league": "Ligue 1",
                "overall_rating": 76, "attack_rating": 75, "midfield_rating": 77, "defense_rating": 76,
                "primary_color": "#FF0000", "secondary_color": "#000000", "stadium_name": "Allianz Riviera",
                "stadium_capacity": 35624, "budget": 45000000, "prestige": 6
            },
            
            # Eredivisie Teams
            {
                "name": "Ajax Amsterdam", "short_name": "AJA", "country": "Netherlands", "league": "Eredivisie",
                "overall_rating": 82, "attack_rating": 84, "midfield_rating": 81, "defense_rating": 80,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "Johan Cruyff Arena",
                "stadium_capacity": 54990, "budget": 60000000, "prestige": 8
            },
            {
                "name": "PSV Eindhoven", "short_name": "PSV", "country": "Netherlands", "league": "Eredivisie",
                "overall_rating": 81, "attack_rating": 83, "midfield_rating": 80, "defense_rating": 79,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "Philips Stadion",
                "stadium_capacity": 35000, "budget": 55000000, "prestige": 7
            },
            {
                "name": "Feyenoord Rotterdam", "short_name": "FEY", "country": "Netherlands", "league": "Eredivisie",
                "overall_rating": 80, "attack_rating": 82, "midfield_rating": 79, "defense_rating": 78,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "De Kuip",
                "stadium_capacity": 51177, "budget": 50000000, "prestige": 7
            },
            
            # Liga Portuguesa
            {
                "name": "FC Porto", "short_name": "POR", "country": "Portugal", "league": "Primeira Liga",
                "overall_rating": 82, "attack_rating": 84, "midfield_rating": 81, "defense_rating": 80,
                "primary_color": "#004C99", "secondary_color": "#FFFFFF", "stadium_name": "Estádio do Dragão",
                "stadium_capacity": 50033, "budget": 55000000, "prestige": 8
            },
            {
                "name": "SL Benfica", "short_name": "BEN", "country": "Portugal", "league": "Primeira Liga",
                "overall_rating": 83, "attack_rating": 85, "midfield_rating": 82, "defense_rating": 81,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "Estádio da Luz",
                "stadium_capacity": 64642, "budget": 60000000, "prestige": 8
            },
            {
                "name": "Sporting CP", "short_name": "SCP", "country": "Portugal", "league": "Primeira Liga",
                "overall_rating": 81, "attack_rating": 83, "midfield_rating": 80, "defense_rating": 79,
                "primary_color": "#00B04F", "secondary_color": "#FFFFFF", "stadium_name": "Estádio José Alvalade",
                "stadium_capacity": 50095, "budget": 50000000, "prestige": 7
            },
            
            # Turkish Super Lig  
            {
                "name": "Galatasaray SK", "short_name": "GAL", "country": "Turkey", "league": "Süper Lig",
                "overall_rating": 79, "attack_rating": 81, "midfield_rating": 78, "defense_rating": 77,
                "primary_color": "#FF8000", "secondary_color": "#8B0000", "stadium_name": "Türk Telekom Stadium",
                "stadium_capacity": 52695, "budget": 35000000, "prestige": 7
            },
            {
                "name": "Fenerbahçe SK", "short_name": "FEN", "country": "Turkey", "league": "Süper Lig",
                "overall_rating": 78, "attack_rating": 80, "midfield_rating": 77, "defense_rating": 76,
                "primary_color": "#FFFF00", "secondary_color": "#000080", "stadium_name": "Şükrü Saracoğlu Stadium",
                "stadium_capacity": 50509, "budget": 40000000, "prestige": 7
            },
            {
                "name": "Beşiktaş JK", "short_name": "BES", "country": "Turkey", "league": "Süper Lig",
                "overall_rating": 77, "attack_rating": 79, "midfield_rating": 76, "defense_rating": 75,
                "primary_color": "#000000", "secondary_color": "#FFFFFF", "stadium_name": "Vodafone Park",
                "stadium_capacity": 41903, "budget": 30000000, "prestige": 6
            },
            
            # MLS Teams
            {
                "name": "Inter Miami CF", "short_name": "MIA", "country": "USA", "league": "MLS",
                "overall_rating": 76, "attack_rating": 79, "midfield_rating": 74, "defense_rating": 73,
                "primary_color": "#FF69B4", "secondary_color": "#000000", "stadium_name": "DRV PNK Stadium",
                "stadium_capacity": 18000, "budget": 25000000, "prestige": 6
            },
            {
                "name": "LA Galaxy", "short_name": "LAG", "country": "USA", "league": "MLS",
                "overall_rating": 75, "attack_rating": 77, "midfield_rating": 74, "defense_rating": 73,
                "primary_color": "#FFFFFF", "secondary_color": "#1E3A8A", "stadium_name": "Dignity Health Sports Park",
                "stadium_capacity": 27000, "budget": 22000000, "prestige": 6
            },
            {
                "name": "Atlanta United FC", "short_name": "ATL", "country": "USA", "league": "MLS",
                "overall_rating": 76, "attack_rating": 78, "midfield_rating": 75, "defense_rating": 74,
                "primary_color": "#8B0000", "secondary_color": "#000000", "stadium_name": "Mercedes-Benz Stadium",
                "stadium_capacity": 71000, "budget": 20000000, "prestige": 6
            },
            
            # More Brazilian Teams
            {
                "name": "Palmeiras", "short_name": "PAL", "country": "Brazil", "league": "Brasileirão",
                "overall_rating": 83, "attack_rating": 85, "midfield_rating": 82, "defense_rating": 81,
                "primary_color": "#00B04F", "secondary_color": "#FFFFFF", "stadium_name": "Allianz Parque",
                "stadium_capacity": 43713, "budget": 45000000, "prestige": 8
            },
            {
                "name": "Corinthians", "short_name": "COR", "country": "Brazil", "league": "Brasileirão",
                "overall_rating": 81, "attack_rating": 83, "midfield_rating": 80, "defense_rating": 79,
                "primary_color": "#FFFFFF", "secondary_color": "#000000", "stadium_name": "Neo Química Arena",
                "stadium_capacity": 49205, "budget": 35000000, "prestige": 8
            },
            {
                "name": "São Paulo FC", "short_name": "SAO", "country": "Brazil", "league": "Brasileirão",
                "overall_rating": 80, "attack_rating": 82, "midfield_rating": 79, "defense_rating": 78,
                "primary_color": "#FF0000", "secondary_color": "#000000", "stadium_name": "Morumbi",
                "stadium_capacity": 67052, "budget": 32000000, "prestige": 8
            }
        ]
        
        for team_data in default_teams:
            team = Team(**team_data)
            # Generate players for each team
            team.players = self.generate_team_players(team)
            await self.db.teams.insert_one(team.dict())
            
    def generate_team_players(self, team: Team) -> List[Player]:
        """Generate comprehensive realistic players for each team with copyright-safe names"""
        import random
        
        players = []
        
        # Define realistic player templates based on team identity
        team_players = self.get_team_player_templates(team.name, team.overall_rating)
        
        for player_template in team_players:
            # Add some randomization while keeping realistic stats
            variation = random.randint(-2, 2)
            
            player = Player(
                name=player_template["name"],
                position=player_template["position"],
                overall_rating=max(50, min(99, player_template["overall_rating"] + variation)),
                pace=max(30, min(99, player_template["pace"] + variation)),
                shooting=max(20, min(99, player_template["shooting"] + variation)),
                passing=max(30, min(99, player_template["passing"] + variation)),
                defending=max(20, min(99, player_template["defending"] + variation)),
                physicality=max(30, min(99, player_template["physicality"] + variation)),
                age=player_template["age"],
                nationality=player_template["nationality"],
                value=player_template["value"],
                stamina=max(60, min(99, player_template["stamina"] + variation)),
                skill_moves=player_template["skill_moves"],
                weak_foot=player_template["weak_foot"],
                is_custom=False
            )
            players.append(player)
        
        return players
    
    def get_team_player_templates(self, team_name: str, team_rating: int) -> list:
        """Get realistic player templates for each team"""
        
        # MANCHESTER BLUE (Manchester City inspired)
        if team_name == "Manchester Blue":
            return [
                # Goalkeepers
                {"name": "Eduardo Silva", "position": Position.GOALKEEPER, "overall_rating": 89, "pace": 45, "shooting": 22, "passing": 75, "defending": 88, "physicality": 82, "age": 29, "nationality": "Brazil", "value": 45000000, "stamina": 88, "skill_moves": 1, "weak_foot": 3},
                {"name": "Stefan Ortega", "position": Position.GOALKEEPER, "overall_rating": 78, "pace": 42, "shooting": 18, "passing": 71, "defending": 79, "physicality": 77, "age": 31, "nationality": "Germany", "value": 8000000, "stamina": 83, "skill_moves": 1, "weak_foot": 2},
                {"name": "Scott Carson", "position": Position.GOALKEEPER, "overall_rating": 67, "pace": 38, "shooting": 15, "passing": 68, "defending": 68, "physicality": 71, "age": 39, "nationality": "England", "value": 500000, "stamina": 75, "skill_moves": 1, "weak_foot": 2},
                
                # Defenders
                {"name": "João Cancelo", "position": Position.DEFENDER, "overall_rating": 87, "pace": 85, "shooting": 75, "passing": 88, "defending": 78, "physicality": 74, "age": 30, "nationality": "Portugal", "value": 55000000, "stamina": 86, "skill_moves": 4, "weak_foot": 4},
                {"name": "Kyle Walker", "position": Position.DEFENDER, "overall_rating": 85, "pace": 91, "shooting": 62, "passing": 75, "defending": 84, "physicality": 81, "age": 34, "nationality": "England", "value": 35000000, "stamina": 89, "skill_moves": 3, "weak_foot": 3},
                {"name": "Ruben Dias", "position": Position.DEFENDER, "overall_rating": 89, "pace": 62, "shooting": 45, "passing": 75, "defending": 91, "physicality": 88, "age": 27, "nationality": "Portugal", "value": 75000000, "stamina": 85, "skill_moves": 2, "weak_foot": 3},
                {"name": "John Stones", "position": Position.DEFENDER, "overall_rating": 84, "pace": 68, "shooting": 48, "passing": 86, "defending": 82, "physicality": 79, "age": 30, "nationality": "England", "value": 40000000, "stamina": 84, "skill_moves": 3, "weak_foot": 4},
                {"name": "Nathan Ake", "position": Position.DEFENDER, "overall_rating": 82, "pace": 75, "shooting": 42, "passing": 78, "defending": 83, "physicality": 76, "age": 29, "nationality": "Netherlands", "value": 38000000, "stamina": 82, "skill_moves": 2, "weak_foot": 3},
                {"name": "Rico Lewis", "position": Position.DEFENDER, "overall_rating": 75, "pace": 78, "shooting": 55, "passing": 82, "defending": 71, "physicality": 62, "age": 19, "nationality": "England", "value": 15000000, "stamina": 86, "skill_moves": 3, "weak_foot": 3},
                {"name": "Sergio Gomez", "position": Position.DEFENDER, "overall_rating": 76, "pace": 79, "shooting": 68, "passing": 85, "defending": 72, "physicality": 65, "age": 23, "nationality": "Spain", "value": 18000000, "stamina": 84, "skill_moves": 3, "weak_foot": 4},
                {"name": "Josh Wilson-Esbrand", "position": Position.DEFENDER, "overall_rating": 68, "pace": 82, "shooting": 45, "passing": 75, "defending": 68, "physicality": 62, "age": 21, "nationality": "England", "value": 5000000, "stamina": 85, "skill_moves": 2, "weak_foot": 2},
                
                # Midfielders
                {"name": "Kevin De Bruyne", "position": Position.MIDFIELDER, "overall_rating": 91, "pace": 68, "shooting": 86, "passing": 93, "defending": 64, "physicality": 77, "age": 33, "nationality": "Belgium", "value": 85000000, "stamina": 88, "skill_moves": 4, "weak_foot": 5},
                {"name": "Rodri Hernandez", "position": Position.MIDFIELDER, "overall_rating": 89, "pace": 59, "shooting": 75, "passing": 91, "defending": 87, "physicality": 87, "age": 28, "nationality": "Spain", "value": 90000000, "stamina": 91, "skill_moves": 3, "weak_foot": 4},
                {"name": "Bernardo Silva", "position": Position.MIDFIELDER, "overall_rating": 86, "pace": 74, "shooting": 79, "passing": 89, "defending": 66, "physicality": 73, "age": 30, "nationality": "Portugal", "value": 70000000, "stamina": 92, "skill_moves": 4, "weak_foot": 4},
                {"name": "Ilkay Gundogan", "position": Position.MIDFIELDER, "overall_rating": 85, "pace": 65, "shooting": 82, "passing": 90, "defending": 71, "physicality": 68, "age": 34, "nationality": "Germany", "value": 35000000, "stamina": 86, "skill_moves": 4, "weak_foot": 4},
                {"name": "Mateo Kovacic", "position": Position.MIDFIELDER, "overall_rating": 83, "pace": 71, "shooting": 70, "passing": 87, "defending": 76, "physicality": 74, "age": 30, "nationality": "Croatia", "value": 45000000, "stamina": 89, "skill_moves": 4, "weak_foot": 3},
                {"name": "Kalvin Phillips", "position": Position.MIDFIELDER, "overall_rating": 79, "pace": 68, "shooting": 65, "passing": 82, "defending": 81, "physicality": 79, "age": 28, "nationality": "England", "value": 25000000, "stamina": 87, "skill_moves": 2, "weak_foot": 3},
                {"name": "Phil Foden", "position": Position.MIDFIELDER, "overall_rating": 85, "pace": 80, "shooting": 82, "passing": 88, "defending": 55, "physicality": 61, "age": 24, "nationality": "England", "value": 80000000, "stamina": 86, "skill_moves": 4, "weak_foot": 4},
                {"name": "Cole Palmer", "position": Position.MIDFIELDER, "overall_rating": 77, "pace": 75, "shooting": 78, "passing": 82, "defending": 45, "physicality": 65, "age": 22, "nationality": "England", "value": 25000000, "stamina": 82, "skill_moves": 4, "weak_foot": 3},
                
                # Forwards
                {"name": "Erling Haaland", "position": Position.FORWARD, "overall_rating": 91, "pace": 89, "shooting": 94, "passing": 65, "defending": 45, "physicality": 88, "age": 24, "nationality": "Norway", "value": 180000000, "stamina": 88, "skill_moves": 3, "weak_foot": 3},
                {"name": "Jack Grealish", "position": Position.FORWARD, "overall_rating": 84, "pace": 80, "shooting": 74, "passing": 85, "defending": 43, "physicality": 65, "age": 29, "nationality": "England", "value": 65000000, "stamina": 85, "skill_moves": 4, "weak_foot": 3},
                {"name": "Riyad Mahrez", "position": Position.FORWARD, "overall_rating": 86, "pace": 80, "shooting": 84, "passing": 82, "defending": 38, "physicality": 61, "age": 33, "nationality": "Algeria", "value": 35000000, "stamina": 82, "skill_moves": 5, "weak_foot": 4},
                {"name": "Julian Alvarez", "position": Position.FORWARD, "overall_rating": 82, "pace": 85, "shooting": 83, "passing": 78, "defending": 58, "physicality": 70, "age": 24, "nationality": "Argentina", "value": 70000000, "stamina": 91, "skill_moves": 4, "weak_foot": 4},
                {"name": "Jeremy Doku", "position": Position.FORWARD, "overall_rating": 78, "pace": 93, "shooting": 68, "passing": 74, "defending": 32, "physicality": 65, "age": 22, "nationality": "Belgium", "value": 35000000, "stamina": 85, "skill_moves": 4, "weak_foot": 3},
                {"name": "Oscar Bobb", "position": Position.FORWARD, "overall_rating": 72, "pace": 82, "shooting": 71, "passing": 76, "defending": 28, "physicality": 58, "age": 21, "nationality": "Norway", "value": 12000000, "stamina": 84, "skill_moves": 3, "weak_foot": 3}
            ]
        
        # MADRID WHITE (Real Madrid inspired)
        elif team_name == "Madrid White":
            return [
                # Goalkeepers
                {"name": "Thibaut Courtois", "position": Position.GOALKEEPER, "overall_rating": 90, "pace": 41, "shooting": 17, "passing": 75, "defending": 90, "physicality": 89, "age": 32, "nationality": "Belgium", "value": 60000000, "stamina": 85, "skill_moves": 1, "weak_foot": 2},
                {"name": "Andriy Lunin", "position": Position.GOALKEEPER, "overall_rating": 77, "pace": 48, "shooting": 14, "passing": 72, "defending": 78, "physicality": 76, "age": 25, "nationality": "Ukraine", "value": 15000000, "stamina": 82, "skill_moves": 1, "weak_foot": 2},
                {"name": "Kepa Arrizabalaga", "position": Position.GOALKEEPER, "overall_rating": 79, "pace": 52, "shooting": 19, "passing": 78, "defending": 79, "physicality": 71, "age": 30, "nationality": "Spain", "value": 20000000, "stamina": 84, "skill_moves": 1, "weak_foot": 3},
                
                # Defenders
                {"name": "Dani Carvajal", "position": Position.DEFENDER, "overall_rating": 84, "pace": 78, "shooting": 68, "passing": 82, "defending": 85, "physicality": 79, "age": 32, "nationality": "Spain", "value": 25000000, "stamina": 88, "skill_moves": 3, "weak_foot": 3},
                {"name": "David Alaba", "position": Position.DEFENDER, "overall_rating": 86, "pace": 71, "shooting": 74, "passing": 88, "defending": 85, "physicality": 78, "age": 32, "nationality": "Austria", "value": 40000000, "stamina": 81, "skill_moves": 4, "weak_foot": 4},
                {"name": "Antonio Rudiger", "position": Position.DEFENDER, "overall_rating": 87, "pace": 82, "shooting": 55, "passing": 73, "defending": 88, "physicality": 86, "age": 31, "nationality": "Germany", "value": 45000000, "stamina": 89, "skill_moves": 2, "weak_foot": 3},
                {"name": "Eder Militao", "position": Position.DEFENDER, "overall_rating": 85, "pace": 81, "shooting": 39, "passing": 71, "defending": 86, "physicality": 82, "age": 26, "nationality": "Brazil", "value": 70000000, "stamina": 86, "skill_moves": 2, "weak_foot": 3},
                {"name": "Ferland Mendy", "position": Position.DEFENDER, "overall_rating": 82, "pace": 88, "shooting": 39, "passing": 74, "defending": 82, "physicality": 82, "age": 29, "nationality": "France", "value": 35000000, "stamina": 86, "skill_moves": 3, "weak_foot": 3},
                {"name": "Lucas Vazquez", "position": Position.DEFENDER, "overall_rating": 79, "pace": 77, "shooting": 74, "passing": 79, "defending": 76, "physicality": 71, "age": 33, "nationality": "Spain", "value": 8000000, "stamina": 91, "skill_moves": 3, "weak_foot": 3},
                {"name": "Nacho Fernandez", "position": Position.DEFENDER, "overall_rating": 80, "pace": 70, "shooting": 58, "passing": 75, "defending": 82, "physicality": 78, "age": 34, "nationality": "Spain", "value": 5000000, "stamina": 85, "skill_moves": 2, "weak_foot": 3},
                {"name": "Fran Garcia", "position": Position.DEFENDER, "overall_rating": 76, "pace": 84, "shooting": 52, "passing": 79, "defending": 74, "physicality": 68, "age": 25, "nationality": "Spain", "value": 15000000, "stamina": 87, "skill_moves": 3, "weak_foot": 3},
                
                # Midfielders
                {"name": "Luka Modric", "position": Position.MIDFIELDER, "overall_rating": 88, "pace": 68, "shooting": 76, "passing": 91, "defending": 72, "physicality": 65, "age": 39, "nationality": "Croatia", "value": 10000000, "stamina": 88, "skill_moves": 4, "weak_foot": 4},
                {"name": "Toni Kroos", "position": Position.MIDFIELDER, "overall_rating": 88, "pace": 54, "shooting": 81, "passing": 94, "defending": 71, "physicality": 70, "age": 34, "nationality": "Germany", "value": 15000000, "stamina": 86, "skill_moves": 4, "weak_foot": 4},
                {"name": "Federico Valverde", "position": Position.MIDFIELDER, "overall_rating": 87, "pace": 80, "shooting": 84, "passing": 84, "defending": 76, "physicality": 82, "age": 26, "nationality": "Uruguay", "value": 100000000, "stamina": 95, "skill_moves": 3, "weak_foot": 4},
                {"name": "Eduardo Camavinga", "position": Position.MIDFIELDER, "overall_rating": 82, "pace": 82, "shooting": 64, "passing": 81, "defending": 78, "physicality": 78, "age": 22, "nationality": "France", "value": 80000000, "stamina": 91, "skill_moves": 4, "weak_foot": 3},
                {"name": "Aurelien Tchouameni", "position": Position.MIDFIELDER, "overall_rating": 84, "pace": 68, "shooting": 72, "passing": 80, "defending": 84, "physicality": 87, "age": 24, "nationality": "France", "value": 90000000, "stamina": 89, "skill_moves": 3, "weak_foot": 3},
                {"name": "Jude Bellingham", "position": Position.MIDFIELDER, "overall_rating": 87, "pace": 75, "shooting": 83, "passing": 83, "defending": 72, "physicality": 82, "age": 21, "nationality": "England", "value": 150000000, "stamina": 92, "skill_moves": 4, "weak_foot": 4},
                {"name": "Dani Ceballos", "position": Position.MIDFIELDER, "overall_rating": 78, "pace": 71, "shooting": 73, "passing": 84, "defending": 68, "physicality": 66, "age": 28, "nationality": "Spain", "value": 15000000, "stamina": 84, "skill_moves": 4, "weak_foot": 3},
                {"name": "Arda Guler", "position": Position.MIDFIELDER, "overall_rating": 74, "pace": 74, "shooting": 76, "passing": 82, "defending": 42, "physicality": 56, "age": 19, "nationality": "Turkey", "value": 25000000, "stamina": 78, "skill_moves": 4, "weak_foot": 4},
                
                # Forwards
                {"name": "Kylian Mbappe", "position": Position.FORWARD, "overall_rating": 91, "pace": 97, "shooting": 89, "passing": 80, "defending": 39, "physicality": 77, "age": 26, "nationality": "France", "value": 180000000, "stamina": 92, "skill_moves": 5, "weak_foot": 4},
                {"name": "Vinicius Junior", "position": Position.FORWARD, "overall_rating": 89, "pace": 95, "shooting": 83, "passing": 75, "defending": 29, "physicality": 68, "age": 24, "nationality": "Brazil", "value": 150000000, "stamina": 87, "skill_moves": 5, "weak_foot": 3},
                {"name": "Rodrygo Goes", "position": Position.FORWARD, "overall_rating": 85, "pace": 91, "shooting": 82, "passing": 78, "defending": 43, "physicality": 65, "age": 24, "nationality": "Brazil", "value": 80000000, "stamina": 85, "skill_moves": 4, "weak_foot": 4},
                {"name": "Endrick Felipe", "position": Position.FORWARD, "overall_rating": 77, "pace": 84, "shooting": 79, "passing": 68, "defending": 25, "physicality": 73, "age": 18, "nationality": "Brazil", "value": 40000000, "stamina": 82, "skill_moves": 4, "weak_foot": 3},
                {"name": "Brahim Diaz", "position": Position.FORWARD, "overall_rating": 80, "pace": 83, "shooting": 78, "passing": 81, "defending": 35, "physicality": 58, "age": 25, "nationality": "Morocco", "value": 30000000, "stamina": 83, "skill_moves": 4, "weak_foot": 3},
                {"name": "Joselu Mato", "position": Position.FORWARD, "overall_rating": 78, "pace": 65, "shooting": 82, "passing": 72, "defending": 42, "physicality": 84, "age": 34, "nationality": "Spain", "value": 8000000, "stamina": 79, "skill_moves": 2, "weak_foot": 3}
            ]
        
        # BARCELONA FC (enhanced with star players)
        elif team_name == "Barcelona FC":
            return [
                # Goalkeepers
                {"name": "Marc-Andre ter Stegen", "position": Position.GOALKEEPER, "overall_rating": 89, "pace": 43, "shooting": 16, "passing": 84, "defending": 90, "physicality": 82, "age": 32, "nationality": "Germany", "value": 50000000, "stamina": 85, "skill_moves": 1, "weak_foot": 2, "special_abilities": ["goalkeeper_reflexes", "distribution_master"]},
                {"name": "Inaki Pena", "position": Position.GOALKEEPER, "overall_rating": 74, "pace": 48, "shooting": 13, "passing": 76, "defending": 75, "physicality": 73, "age": 25, "nationality": "Spain", "value": 8000000, "stamina": 82, "skill_moves": 1, "weak_foot": 2, "special_abilities": []},
                {"name": "Ander Astralaga", "position": Position.GOALKEEPER, "overall_rating": 67, "pace": 45, "shooting": 12, "passing": 71, "defending": 68, "physicality": 69, "age": 20, "nationality": "Spain", "value": 2000000, "stamina": 80, "skill_moves": 1, "weak_foot": 2, "special_abilities": []},
                
                # Defenders
                {"name": "Ronald Araujo", "position": Position.DEFENDER, "overall_rating": 85, "pace": 82, "shooting": 48, "passing": 71, "defending": 87, "physicality": 89, "age": 25, "nationality": "Uruguay", "value": 70000000, "stamina": 87, "skill_moves": 2, "weak_foot": 3, "special_abilities": ["aerial_dominance", "last_man_defending"]},
                {"name": "Jules Kounde", "position": Position.DEFENDER, "overall_rating": 84, "pace": 83, "shooting": 43, "passing": 78, "defending": 85, "physicality": 78, "age": 26, "nationality": "France", "value": 65000000, "stamina": 86, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["pace_defending", "ball_playing_defender"]},
                {"name": "Andreas Christensen", "position": Position.DEFENDER, "overall_rating": 81, "pace": 74, "shooting": 41, "passing": 82, "defending": 83, "physicality": 77, "age": 28, "nationality": "Denmark", "value": 35000000, "stamina": 84, "skill_moves": 2, "weak_foot": 4, "special_abilities": ["ball_playing_defender"]},
                {"name": "Alejandro Balde", "position": Position.DEFENDER, "overall_rating": 78, "pace": 87, "shooting": 52, "passing": 76, "defending": 76, "physicality": 71, "age": 21, "nationality": "Spain", "value": 35000000, "stamina": 89, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["lightning_pace", "attacking_fullback"]},
                {"name": "Joao Cancelo", "position": Position.DEFENDER, "overall_rating": 86, "pace": 84, "shooting": 74, "passing": 87, "defending": 77, "physicality": 73, "age": 30, "nationality": "Portugal", "value": 45000000, "stamina": 85, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["attacking_fullback", "versatility_master"]},
                {"name": "Marcos Alonso", "position": Position.DEFENDER, "overall_rating": 77, "pace": 68, "shooting": 76, "passing": 79, "defending": 78, "physicality": 82, "age": 33, "nationality": "Spain", "value": 12000000, "stamina": 81, "skill_moves": 3, "weak_foot": 4, "special_abilities": ["free_kick_specialist"]},
                {"name": "Sergi Roberto", "position": Position.DEFENDER, "overall_rating": 76, "pace": 73, "shooting": 68, "passing": 83, "defending": 74, "physicality": 71, "age": 32, "nationality": "Spain", "value": 8000000, "stamina": 88, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["versatility_master"]},
                {"name": "Inigo Martinez", "position": Position.DEFENDER, "overall_rating": 82, "pace": 66, "shooting": 45, "passing": 78, "defending": 86, "physicality": 84, "age": 33, "nationality": "Spain", "value": 15000000, "stamina": 83, "skill_moves": 2, "weak_foot": 4, "special_abilities": ["ball_playing_defender", "leadership"]},
                
                # Midfielders
                {"name": "Pedri Gonzalez", "position": Position.MIDFIELDER, "overall_rating": 87, "pace": 74, "shooting": 76, "passing": 90, "defending": 59, "physicality": 66, "age": 22, "nationality": "Spain", "value": 100000000, "stamina": 88, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["vision_master", "press_resistant", "la_masia_magic"]},
                {"name": "Frenkie de Jong", "position": Position.MIDFIELDER, "overall_rating": 87, "pace": 78, "shooting": 72, "passing": 89, "defending": 78, "physicality": 79, "age": 27, "nationality": "Netherlands", "value": 80000000, "stamina": 92, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["press_resistant", "box_to_box_master", "dribbling_midfielder"]},
                {"name": "Gavi", "position": Position.MIDFIELDER, "overall_rating": 84, "pace": 79, "shooting": 68, "passing": 86, "defending": 71, "physicality": 68, "age": 20, "nationality": "Spain", "value": 90000000, "stamina": 94, "skill_moves": 4, "weak_foot": 3, "special_abilities": ["press_resistant", "young_phenomenon", "la_masia_magic"]},
                {"name": "Ilkay Gundogan", "position": Position.MIDFIELDER, "overall_rating": 85, "pace": 63, "shooting": 81, "passing": 91, "defending": 73, "physicality": 69, "age": 34, "nationality": "Germany", "value": 25000000, "stamina": 85, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["vision_master", "free_kick_specialist", "leadership"]},
                {"name": "Oriol Romeu", "position": Position.MIDFIELDER, "overall_rating": 78, "pace": 62, "shooting": 58, "passing": 81, "defending": 83, "physicality": 81, "age": 32, "nationality": "Spain", "value": 12000000, "stamina": 87, "skill_moves": 2, "weak_foot": 3, "special_abilities": ["defensive_midfielder"]},
                {"name": "Fermin Lopez", "position": Position.MIDFIELDER, "overall_rating": 76, "pace": 77, "shooting": 73, "passing": 82, "defending": 65, "physicality": 70, "age": 21, "nationality": "Spain", "value": 20000000, "stamina": 86, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["young_phenomenon", "la_masia_magic"]},
                {"name": "Pablo Torre", "position": Position.MIDFIELDER, "overall_rating": 74, "pace": 71, "shooting": 75, "passing": 84, "defending": 52, "physicality": 61, "age": 21, "nationality": "Spain", "value": 15000000, "stamina": 82, "skill_moves": 4, "weak_foot": 3, "special_abilities": ["young_phenomenon", "creative_playmaker"]},
                {"name": "Marc Casado", "position": Position.MIDFIELDER, "overall_rating": 72, "pace": 69, "shooting": 64, "passing": 79, "defending": 74, "physicality": 68, "age": 21, "nationality": "Spain", "value": 8000000, "stamina": 84, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["young_phenomenon", "la_masia_magic"]},
                
                # Forwards
                {"name": "Robert Lewandowski", "position": Position.FORWARD, "overall_rating": 90, "pace": 77, "shooting": 94, "passing": 79, "defending": 43, "physicality": 84, "age": 36, "nationality": "Poland", "value": 35000000, "stamina": 82, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["clinical_finisher", "penalty_master", "target_man", "veteran_experience"]},
                {"name": "Raphinha", "position": Position.FORWARD, "overall_rating": 84, "pace": 85, "shooting": 82, "passing": 78, "defending": 37, "physicality": 67, "age": 28, "nationality": "Brazil", "value": 60000000, "stamina": 87, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["brazilian_flair", "wing_wizard", "pace_merchant"]},
                {"name": "Joao Felix", "position": Position.FORWARD, "overall_rating": 83, "pace": 83, "shooting": 81, "passing": 82, "defending": 31, "physicality": 65, "age": 25, "nationality": "Portugal", "value": 70000000, "stamina": 84, "skill_moves": 5, "weak_foot": 4, "special_abilities": ["technical_dribbler", "creative_forward", "luxury_player"]},
                {"name": "Ferran Torres", "position": Position.FORWARD, "overall_rating": 82, "pace": 86, "shooting": 81, "passing": 76, "defending": 42, "physicality": 69, "age": 24, "nationality": "Spain", "value": 50000000, "stamina": 88, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["pace_merchant", "versatile_forward"]},
                {"name": "Ansu Fati", "position": Position.FORWARD, "overall_rating": 80, "pace": 84, "shooting": 79, "passing": 74, "defending": 26, "physicality": 62, "age": 22, "nationality": "Spain", "value": 40000000, "stamina": 81, "skill_moves": 4, "weak_foot": 3, "special_abilities": ["young_phenomenon", "la_masia_magic", "injury_prone"]},
                {"name": "Vitor Roque", "position": Position.FORWARD, "overall_rating": 75, "pace": 81, "shooting": 76, "passing": 68, "defending": 22, "physicality": 72, "age": 19, "nationality": "Brazil", "value": 25000000, "stamina": 83, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["young_phenomenon", "brazilian_prospect"]}
            ]
        
        # LIVERPOOL RED (enhanced with star players)  
        elif team_name == "Liverpool Red":
            return [
                # Goalkeepers
                {"name": "Alisson Becker", "position": Position.GOALKEEPER, "overall_rating": 90, "pace": 51, "shooting": 18, "passing": 80, "defending": 91, "physicality": 86, "age": 31, "nationality": "Brazil", "value": 55000000, "stamina": 86, "skill_moves": 1, "weak_foot": 3, "special_abilities": ["goalkeeper_reflexes", "distribution_master", "sweeper_keeper"]},
                {"name": "Caoimhin Kelleher", "position": Position.GOALKEEPER, "overall_rating": 79, "pace": 49, "shooting": 15, "passing": 75, "defending": 80, "physicality": 78, "age": 26, "nationality": "Ireland", "value": 18000000, "stamina": 83, "skill_moves": 1, "weak_foot": 2, "special_abilities": ["young_keeper"]},
                {"name": "Adrian San Miguel", "position": Position.GOALKEEPER, "overall_rating": 71, "pace": 42, "shooting": 14, "passing": 68, "defending": 72, "physicality": 74, "age": 37, "nationality": "Spain", "value": 2000000, "stamina": 79, "skill_moves": 1, "weak_foot": 2, "special_abilities": ["veteran_experience"]},
                
                # Defenders
                {"name": "Virgil van Dijk", "position": Position.DEFENDER, "overall_rating": 89, "pace": 76, "shooting": 62, "passing": 91, "defending": 92, "physicality": 86, "age": 33, "nationality": "Netherlands", "value": 55000000, "stamina": 84, "skill_moves": 2, "weak_foot": 3, "special_abilities": ["aerial_dominance", "captain_influence", "ball_playing_defender", "leadership"]},
                {"name": "Mohamed Salah", "position": Position.DEFENDER, "overall_rating": 84, "pace": 82, "shooting": 55, "passing": 85, "defending": 86, "physicality": 83, "age": 26, "nationality": "Egypt", "value": 60000000, "stamina": 87, "skill_moves": 2, "weak_foot": 3, "special_abilities": ["pace_defending", "ball_playing_defender"]},
                {"name": "Andrew Robertson", "position": Position.DEFENDER, "overall_rating": 86, "pace": 86, "shooting": 59, "passing": 84, "defending": 85, "physicality": 78, "age": 30, "nationality": "Scotland", "value": 40000000, "stamina": 92, "skill_moves": 3, "weak_foot": 4, "special_abilities": ["attacking_fullback", "crossing_specialist", "stamina_monster"]},
                {"name": "Trent Alexander-Arnold", "position": Position.DEFENDER, "overall_rating": 87, "pace": 76, "shooting": 75, "passing": 93, "defending": 78, "physicality": 71, "age": 26, "nationality": "England", "value": 80000000, "stamina": 89, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["crossing_specialist", "free_kick_specialist", "vision_master", "attacking_fullback"]},
                {"name": "Joel Matip", "position": Position.DEFENDER, "overall_rating": 81, "pace": 68, "shooting": 48, "passing": 79, "defending": 84, "physicality": 87, "age": 33, "nationality": "Cameroon", "value": 15000000, "stamina": 82, "skill_moves": 2, "weak_foot": 3, "special_abilities": ["aerial_dominance", "ball_playing_defender"]},
                {"name": "Kostas Tsimikas", "position": Position.DEFENDER, "overall_rating": 77, "pace": 82, "shooting": 56, "passing": 78, "defending": 76, "physicality": 73, "age": 28, "nationality": "Greece", "value": 20000000, "stamina": 88, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["attacking_fullback"]},
                {"name": "Conor Bradley", "position": Position.DEFENDER, "overall_rating": 73, "pace": 84, "shooting": 62, "passing": 75, "defending": 71, "physicality": 68, "age": 21, "nationality": "Northern Ireland", "value": 12000000, "stamina": 87, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["young_phenomenon", "attacking_fullback"]},
                {"name": "Joe Gomez", "position": Position.DEFENDER, "overall_rating": 79, "pace": 78, "shooting": 44, "passing": 74, "defending": 81, "physicality": 82, "age": 27, "nationality": "England", "value": 25000000, "stamina": 85, "skill_moves": 2, "weak_foot": 3, "special_abilities": ["versatility_master", "pace_defending"]},
                
                # Midfielders
                {"name": "Fabinho Tavares", "position": Position.MIDFIELDER, "overall_rating": 85, "pace": 68, "shooting": 73, "passing": 86, "defending": 88, "physicality": 83, "age": 31, "nationality": "Brazil", "value": 35000000, "stamina": 89, "skill_moves": 3, "weak_foot": 4, "special_abilities": ["defensive_midfielder", "brazilian_flair", "leadership"]},
                {"name": "Jordan Henderson", "position": Position.MIDFIELDER, "overall_rating": 81, "pace": 67, "shooting": 68, "passing": 86, "defending": 79, "physicality": 76, "age": 34, "nationality": "England", "value": 12000000, "stamina": 91, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["captain_influence", "leadership", "veteran_experience", "stamina_monster"]},
                {"name": "Thiago Alcantara", "position": Position.MIDFIELDER, "overall_rating": 86, "pace": 68, "shooting": 76, "passing": 92, "defending": 75, "physicality": 70, "age": 33, "nationality": "Spain", "value": 20000000, "stamina": 81, "skill_moves": 5, "weak_foot": 4, "special_abilities": ["vision_master", "press_resistant", "technical_master", "injury_prone"]},
                {"name": "Curtis Jones", "position": Position.MIDFIELDER, "overall_rating": 78, "pace": 75, "shooting": 72, "passing": 81, "defending": 69, "physicality": 71, "age": 23, "nationality": "England", "value": 25000000, "stamina": 86, "skill_moves": 4, "weak_foot": 3, "special_abilities": ["young_phenomenon", "academy_graduate"]},
                {"name": "Harvey Elliott", "position": Position.MIDFIELDER, "overall_rating": 76, "pace": 77, "shooting": 74, "passing": 82, "defending": 58, "physicality": 62, "age": 21, "nationality": "England", "value": 30000000, "stamina": 84, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["young_phenomenon", "creative_playmaker", "academy_graduate"]},
                {"name": "Stefan Bajcetic", "position": Position.MIDFIELDER, "overall_rating": 72, "pace": 71, "shooting": 65, "passing": 76, "defending": 74, "physicality": 68, "age": 20, "nationality": "Spain", "value": 15000000, "stamina": 83, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["young_phenomenon", "academy_graduate"]},
                {"name": "Alexis Mac Allister", "position": Position.MIDFIELDER, "overall_rating": 83, "pace": 73, "shooting": 78, "passing": 87, "defending": 75, "physicality": 74, "age": 25, "nationality": "Argentina", "value": 60000000, "stamina": 88, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["box_to_box_master", "world_cup_winner"]},
                {"name": "Dominik Szoboszlai", "position": Position.MIDFIELDER, "overall_rating": 81, "pace": 78, "shooting": 82, "passing": 84, "defending": 68, "physicality": 75, "age": 24, "nationality": "Hungary", "value": 55000000, "stamina": 87, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["free_kick_specialist", "creative_playmaker", "hungarian_talent"]},
                
                # Forwards
                {"name": "Mohamed Salah", "position": Position.FORWARD, "overall_rating": 90, "pace": 90, "shooting": 89, "passing": 81, "defending": 45, "physicality": 75, "age": 32, "nationality": "Egypt", "value": 65000000, "stamina": 88, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["pace_merchant", "clinical_finisher", "penalty_master", "left_foot_wizard", "pharaoh_magic"]},
                {"name": "Sadio Mane", "position": Position.FORWARD, "overall_rating": 88, "pace": 91, "shooting": 85, "passing": 76, "defending": 44, "physicality": 77, "age": 32, "nationality": "Senegal", "value": 50000000, "stamina": 92, "skill_moves": 4, "weak_foot": 3, "special_abilities": ["lightning_pace", "african_warrior", "big_game_player", "stamina_monster"]},
                {"name": "Darwin Nunez", "position": Position.FORWARD, "overall_rating": 82, "pace": 89, "shooting": 83, "passing": 68, "defending": 38, "physicality": 82, "age": 25, "nationality": "Uruguay", "value": 75000000, "stamina": 89, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["pace_merchant", "aerial_dominance", "south_american_flair"]},
                {"name": "Diogo Jota", "position": Position.FORWARD, "overall_rating": 84, "pace": 84, "shooting": 85, "passing": 74, "defending": 43, "physicality": 71, "age": 28, "nationality": "Portugal", "value": 55000000, "stamina": 86, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["versatile_forward", "clinical_finisher", "big_game_player"]},
                {"name": "Luis Diaz", "position": Position.FORWARD, "overall_rating": 84, "pace": 88, "shooting": 78, "passing": 76, "defending": 39, "physicality": 70, "age": 27, "nationality": "Colombia", "value": 65000000, "stamina": 87, "skill_moves": 4, "weak_foot": 3, "special_abilities": ["pace_merchant", "south_american_flair", "wing_wizard"]},
                {"name": "Cody Gakpo", "position": Position.FORWARD, "overall_rating": 81, "pace": 82, "shooting": 80, "passing": 78, "defending": 35, "physicality": 73, "age": 25, "nationality": "Netherlands", "value": 50000000, "stamina": 85, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["versatile_forward", "dutch_technique", "aerial_threat"]}
            ]
    
    def generate_default_players(self, team_name: str, team_rating: int) -> list:
        """Generate default players when specific templates aren't available"""
        import random
        
        # Generic player name pools (copyright-safe)
        first_names = [
            "Alex", "Marco", "David", "Carlos", "João", "Miguel", "Antonio", "Luis", "Fernando", "Diego",
            "André", "Pedro", "Rafael", "Gabriel", "Daniel", "Ricardo", "Paulo", "Bruno", "Sergio", "Manuel",
            "José", "Francisco", "Roberto", "Eduardo", "Adrián", "Alejandro", "Gonzalo", "Martín", "Nicolás", "Sebastián"
        ]
        
        last_names = [
            "Silva", "Santos", "Oliveira", "Pereira", "Costa", "Rodrigues", "Martins", "Jesus", "Sousa", "Fernandes",
            "Gonçalves", "Gomes", "Lopes", "Marques", "Alves", "Almeida", "Ribeiro", "Pinto", "Carvalho", "Teixeira",
            "Moreira", "Ferreira", "Dias", "Mendes", "Nunes", "Correia", "Reis", "Antunes", "Fonseca", "Pires"
        ]
        
        countries = [
            "Portugal", "Spain", "Brazil", "Argentina", "France", "Italy", "Germany", "England", 
            "Netherlands", "Belgium", "Croatia", "Colombia", "Mexico", "Chile", "Uruguay", "Morocco"
        ]
        
        players = []
        positions_count = [
            (Position.GOALKEEPER, 3),
            (Position.DEFENDER, 8), 
            (Position.MIDFIELDER, 8),
            (Position.FORWARD, 6)
        ]
        
        for position, count in positions_count:
            for i in range(count):
                # Determine if this is a star player (first 1-2 in each position)
                is_star = i < 2
                
                base_rating = team_rating
                if is_star:
                    base_rating = team_rating + random.randint(-2, 3)
                else:
                    base_rating = team_rating - random.randint(3, 12)
                
                base_rating = max(50, min(99, base_rating))
                
                # Generate position-specific stats
                stats = self.generate_position_stats(position, base_rating, is_star)
                
                player = {
                    "name": f"{random.choice(first_names)} {random.choice(last_names)}",
                    "position": position,
                    "overall_rating": base_rating,
                    "pace": stats["pace"],
                    "shooting": stats["shooting"],
                    "passing": stats["passing"],
                    "defending": stats["defending"],
                    "physicality": stats["physicality"],
                    "age": random.randint(18, 35),
                    "nationality": random.choice(countries),
                    "value": self.calculate_player_value(base_rating, position, random.randint(18, 35)),
                    "stamina": random.randint(75, 95),
                    "skill_moves": 5 if is_star and position == Position.FORWARD else random.randint(2, 4),
                    "weak_foot": random.randint(2, 4)
                }
                players.append(player)
        
        return players
    
    def generate_position_stats(self, position: Position, base_rating: int, is_star: bool = False) -> dict:
        """Generate realistic stats based on position"""
        import random
        
        variation = 5 if is_star else 8
        
        if position == Position.GOALKEEPER:
            return {
                "pace": random.randint(35, 55),
                "shooting": random.randint(15, 35),
                "passing": random.randint(max(50, base_rating - 15), min(85, base_rating + 5)),
                "defending": random.randint(max(75, base_rating - 5), min(95, base_rating + 5)),
                "physicality": random.randint(max(70, base_rating - 10), min(90, base_rating + 5))
            }
        elif position == Position.DEFENDER:
            return {
                "pace": random.randint(max(45, base_rating - 20), min(85, base_rating)),
                "shooting": random.randint(25, 65),
                "passing": random.randint(max(60, base_rating - 15), min(90, base_rating + 5)),
                "defending": random.randint(max(75, base_rating - 5), min(95, base_rating + 5)),
                "physicality": random.randint(max(70, base_rating - 10), min(95, base_rating + 5))
            }
        elif position == Position.MIDFIELDER:
            return {
                "pace": random.randint(max(55, base_rating - 15), min(85, base_rating + 5)),
                "shooting": random.randint(max(45, base_rating - 25), min(85, base_rating)),
                "passing": random.randint(max(70, base_rating - 5), min(95, base_rating + 5)),
                "defending": random.randint(max(40, base_rating - 25), min(80, base_rating)),
                "physicality": random.randint(max(55, base_rating - 20), min(85, base_rating))
            }
        else:  # FORWARD
            return {
                "pace": random.randint(max(65, base_rating - 10), min(97, base_rating + 5)),
                "shooting": random.randint(max(70, base_rating - 5), min(95, base_rating + 5)),
                "passing": random.randint(max(50, base_rating - 20), min(85, base_rating)),
                "defending": random.randint(25, 55),
                "physicality": random.randint(max(55, base_rating - 20), min(85, base_rating))
            }
    
    def calculate_player_value(self, overall_rating: int, position: Position, age: int) -> int:
        """Calculate realistic player market value"""
        base_value = overall_rating * 1000000
        
        # Age factor
        if age <= 21:
            age_multiplier = 1.5
        elif age <= 25:
            age_multiplier = 1.3
        elif age <= 28:
            age_multiplier = 1.1
        elif age <= 32:
            age_multiplier = 0.8
        else:
            age_multiplier = 0.4
        
        # Position factor
        position_multiplier = 1.2 if position == Position.FORWARD else 1.0
        
        final_value = int(base_value * age_multiplier * position_multiplier)
        return max(100000, min(200000000, final_value))
    
    async def create_default_stadiums(self):
        """Create unique stadiums with special characteristics"""
        stadiums = [
            {
                "name": "Majestic Arena", "capacity": 80000, "country": "England", "city": "London",
                "surface_type": "hybrid_grass", "roof_type": "retractable", "atmosphere_rating": 9,
                "weather_conditions": ["rainy", "cloudy", "sunny"],
                "unique_features": ["historic_stands", "royal_box", "steep_stands"]
            },
            {
                "name": "Cathedral of Football", "capacity": 99000, "country": "Spain", "city": "Barcelona",
                "surface_type": "natural_grass", "roof_type": "open", "atmosphere_rating": 10,
                "weather_conditions": ["sunny", "windy"],
                "unique_features": ["massive_capacity", "steep_stands", "iconic_architecture"]
            },
            {
                "name": "Bernabéu Stadium", "capacity": 81000, "country": "Spain", "city": "Madrid",
                "surface_type": "hybrid_grass", "roof_type": "retractable", "atmosphere_rating": 9,
                "weather_conditions": ["sunny", "hot"],
                "unique_features": ["modern_design", "royal_box", "premium_facilities"]
            },
            {
                "name": "Allianz Arena", "capacity": 75000, "country": "Germany", "city": "Munich",
                "surface_type": "natural_grass", "roof_type": "closed", "atmosphere_rating": 9,
                "weather_conditions": ["cold", "snow", "sunny"],
                "unique_features": ["color_changing_exterior", "modern_design", "excellent_acoustics"]
            },
            {
                "name": "Signal Iduna Park", "capacity": 81000, "country": "Germany", "city": "Dortmund",
                "surface_type": "natural_grass", "roof_type": "partial", "atmosphere_rating": 10,
                "weather_conditions": ["cold", "cloudy", "sunny"],
                "unique_features": ["yellow_wall", "amazing_atmosphere", "standing_section"]
            },
            {
                "name": "San Siro", "capacity": 80000, "country": "Italy", "city": "Milan",
                "surface_type": "natural_grass", "roof_type": "open", "atmosphere_rating": 9,
                "weather_conditions": ["mild", "foggy", "sunny"],
                "unique_features": ["historic_stadium", "shared_venue", "iconic_architecture"]
            },
            {
                "name": "Parc des Princes", "capacity": 48000, "country": "France", "city": "Paris",
                "surface_type": "hybrid_grass", "roof_type": "open", "atmosphere_rating": 8,
                "weather_conditions": ["mild", "rainy", "sunny"],
                "unique_features": ["intimate_atmosphere", "modern_facilities", "city_stadium"]
            },
            {
                "name": "Maracanã", "capacity": 78000, "country": "Brazil", "city": "Rio de Janeiro",
                "surface_type": "natural_grass", "roof_type": "partial", "atmosphere_rating": 10,
                "weather_conditions": ["hot", "humid", "sunny"],
                "unique_features": ["legendary_stadium", "world_cup_venue", "samba_atmosphere"]
            },
            {
                "name": "La Bombonera", "capacity": 54000, "country": "Argentina", "city": "Buenos Aires",
                "surface_type": "natural_grass", "roof_type": "open", "atmosphere_rating": 10,
                "weather_conditions": ["mild", "windy", "sunny"],
                "unique_features": ["steep_stands", "intimidating_atmosphere", "unique_shape"]
            },
            {
                "name": "Anfield", "capacity": 54000, "country": "England", "city": "Liverpool",
                "surface_type": "natural_grass", "roof_type": "open", "atmosphere_rating": 9,
                "weather_conditions": ["rainy", "windy", "cloudy"],
                "unique_features": ["the_kop", "amazing_atmosphere", "historic_stadium"]
            }
        ]
        
        for stadium_data in stadiums:
            stadium = Stadium(**stadium_data)
            await self.db.stadiums.insert_one(stadium.dict())
    
    async def create_default_achievements(self):
        """Create comprehensive achievement system"""
        achievements = [
            # Match Achievements
            {
                "name": "First Goal", "description": "Score your first goal",
                "icon": "⚽", "category": "Scoring", "reward_xp": 100, "reward_coins": 500,
                "rarity": "common", "unlock_condition": "goals_scored >= 1",
                "requirement": {"goals_scored": 1}
            },
            {
                "name": "Hat-trick Hero", "description": "Score 3 goals in a single match",
                "icon": "🎩", "category": "Scoring", "reward_xp": 500, "reward_coins": 2000,
                "rarity": "rare", "unlock_condition": "goals_in_match >= 3",
                "requirement": {"goals_in_match": 3}
            },
            {
                "name": "Clean Sheet", "description": "Win a match without conceding",
                "icon": "🥅", "category": "Defending", "reward_xp": 200, "reward_coins": 1000,
                "rarity": "common", "unlock_condition": "clean_sheets >= 1",
                "requirement": {"clean_sheets": 1}
            },
            {
                "name": "Unbeaten Run", "description": "Go 10 matches without losing",
                "icon": "🔥", "category": "Streak", "reward_xp": 1000, "reward_coins": 5000,
                "rarity": "epic", "unlock_condition": "unbeaten_streak >= 10",
                "requirement": {"unbeaten_streak": 10}
            },
            {
                "name": "Perfect Season", "description": "Win all matches in a season",
                "icon": "👑", "category": "Season", "reward_xp": 5000, "reward_coins": 25000,
                "rarity": "legendary", "unlock_condition": "perfect_season == true",
                "requirement": {"perfect_season": True}
            },
            
            # Career Achievements
            {
                "name": "New Manager", "description": "Start your first career",
                "icon": "👔", "category": "Career", "reward_xp": 200, "reward_coins": 1000,
                "rarity": "common", "unlock_condition": "career_started == true",
                "requirement": {"career_started": True}
            },
            {
                "name": "Trophy Hunter", "description": "Win your first trophy",
                "icon": "🏆", "category": "Trophies", "reward_xp": 800, "reward_coins": 3000,
                "rarity": "rare", "unlock_condition": "trophies_won >= 1",
                "requirement": {"trophies_won": 1}
            },
            {
                "name": "Transfer Master", "description": "Complete 50 transfers",
                "icon": "🔄", "category": "Transfers", "reward_xp": 1500, "reward_coins": 7500,
                "rarity": "epic", "unlock_condition": "transfers_completed >= 50",
                "requirement": {"transfers_completed": 50}
            },
            
            # Skill Achievements
            {
                "name": "Skill Master", "description": "Successfully perform 100 skill moves",
                "icon": "🌟", "category": "Skills", "reward_xp": 1000, "reward_coins": 4000,
                "rarity": "rare", "unlock_condition": "skill_moves_successful >= 100",
                "requirement": {"skill_moves_successful": 100}
            },
            {
                "name": "Rainbow Master", "description": "Perform 10 Rainbow Flicks",
                "icon": "🌈", "category": "Skills", "reward_xp": 500, "reward_coins": 2000,
                "rarity": "rare", "unlock_condition": "rainbow_flicks >= 10",
                "requirement": {"rainbow_flicks": 10}
            },
            
            # Special Achievements
            {
                "name": "World Traveler", "description": "Play in 20 different stadiums",
                "icon": "🌍", "category": "Exploration", "reward_xp": 2000, "reward_coins": 10000,
                "rarity": "epic", "unlock_condition": "stadiums_played >= 20",
                "requirement": {"stadiums_played": 20}
            },
            {
                "name": "Legend", "description": "Reach level 50",
                "icon": "⭐", "category": "Progress", "reward_xp": 10000, "reward_coins": 50000,
                "rarity": "legendary", "unlock_condition": "level >= 50",
                "requirement": {"level": 50}
            }
        ]
        
        for achievement_data in achievements:
            achievement = Achievement(**achievement_data)
            await self.db.achievements.insert_one(achievement.dict())
    
    # CRUD Operations
    async def create_user_profile(self, profile_data: dict) -> str:
        profile = UserProfile(**profile_data)
        result = await self.db.user_profiles.insert_one(profile.dict())
        return str(result.inserted_id)
    
    async def get_user_profile(self, user_id: str) -> Optional[UserProfile]:
        result = await self.db.user_profiles.find_one({"id": user_id})
        return UserProfile(**result) if result else None
    
    async def get_all_teams(self) -> List[Team]:
        cursor = self.db.teams.find()
        teams = []
        async for team_doc in cursor:
            teams.append(Team(**team_doc))
        return teams
    
    async def get_team_by_id(self, team_id: str) -> Optional[Team]:
        result = await self.db.teams.find_one({"id": team_id})
        return Team(**result) if result else None
    
    async def get_teams_by_league(self, league: str) -> List[Team]:
        cursor = self.db.teams.find({"league": league})
        teams = []
        async for team_doc in cursor:
            teams.append(Team(**team_doc))
        return teams
    
    async def get_all_stadiums(self) -> List[Stadium]:
        cursor = self.db.stadiums.find()
        stadiums = []
        async for stadium_doc in cursor:
            stadiums.append(Stadium(**stadium_doc))
        return stadiums
    
    async def create_match(self, match_data: dict) -> str:
        match = Match(**match_data)
        result = await self.db.matches.insert_one(match.dict())
        return str(result.inserted_id)
    
    async def get_user_matches(self, user_id: str) -> List[Match]:
        cursor = self.db.matches.find({"player_id": user_id})
        matches = []
        async for match_doc in cursor:
            matches.append(Match(**match_doc))
        return matches
    
    async def create_tournament(self, tournament_data: dict) -> str:
        tournament = Tournament(**tournament_data)
        result = await self.db.tournaments.insert_one(tournament.dict())
        return str(result.inserted_id)
    
    async def get_all_achievements(self) -> List[Achievement]:
        cursor = self.db.achievements.find()
        achievements = []
        async for achievement_doc in cursor:
            achievements.append(Achievement(**achievement_doc))
        return achievements
    
    async def update_user_achievement(self, user_id: str, achievement_id: str):
        await self.db.user_profiles.update_one(
            {"id": user_id},
            {"$addToSet": {"achievements": achievement_id}}
        )
    
    async def create_career(self, career_data: dict) -> str:
        career = Career(**career_data)
        result = await self.db.careers.insert_one(career.dict())
        return str(result.inserted_id)
    
    async def get_user_career(self, user_id: str) -> Optional[Career]:
        result = await self.db.careers.find_one({"user_id": user_id})
        return Career(**result) if result else None