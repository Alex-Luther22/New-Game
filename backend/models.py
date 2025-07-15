from pydantic import BaseModel, Field
from typing import List, Optional, Dict, Any
import uuid
from datetime import datetime
from enum import Enum

class Position(str, Enum):
    GOALKEEPER = "goalkeeper"
    DEFENDER = "defender"
    MIDFIELDER = "midfielder"
    FORWARD = "forward"

class Formation(str, Enum):
    F_4_4_2 = "4-4-2"
    F_4_3_3 = "4-3-3"
    F_3_5_2 = "3-5-2"
    F_4_2_3_1 = "4-2-3-1"
    F_5_4_1 = "5-4-1"

class GameMode(str, Enum):
    QUICK_MATCH = "quick_match"
    CAREER = "career"
    TOURNAMENT = "tournament"
    FUTSAL = "futsal"
    ONLINE = "online"

class Player(BaseModel):
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    name: str
    position: Position
    overall_rating: int = Field(ge=1, le=99)
    pace: int = Field(ge=1, le=99)
    shooting: int = Field(ge=1, le=99)
    passing: int = Field(ge=1, le=99)
    defending: int = Field(ge=1, le=99)
    physicality: int = Field(ge=1, le=99)
    age: int = Field(ge=16, le=40)
    nationality: str
    value: int = Field(ge=0)
    stamina: int = Field(ge=1, le=99, default=75)
    skill_moves: int = Field(ge=1, le=5, default=2)
    weak_foot: int = Field(ge=1, le=5, default=3)
    is_custom: bool = False
    created_at: datetime = Field(default_factory=datetime.utcnow)

class Team(BaseModel):
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    name: str
    short_name: str
    country: str
    league: str
    overall_rating: int = Field(ge=1, le=99)
    attack_rating: int = Field(ge=1, le=99)
    midfield_rating: int = Field(ge=1, le=99)
    defense_rating: int = Field(ge=1, le=99)
    players: List[Player] = []
    formation: Formation = Formation.F_4_4_2
    primary_color: str = "#FF0000"
    secondary_color: str = "#FFFFFF"
    stadium_name: str
    stadium_capacity: int = Field(ge=1000, le=100000)
    budget: int = Field(ge=0, default=10000000)
    prestige: int = Field(ge=1, le=10, default=5)
    created_at: datetime = Field(default_factory=datetime.utcnow)

class Stadium(BaseModel):
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    name: str
    capacity: int = Field(ge=1000, le=100000)
    country: str
    city: str
    surface_type: str = "grass"
    roof_type: str = "open"
    atmosphere_rating: int = Field(ge=1, le=10)
    weather_conditions: List[str] = ["sunny", "cloudy", "rainy"]
    unique_features: List[str] = []
    created_at: datetime = Field(default_factory=datetime.utcnow)

class UserProfile(BaseModel):
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    username: str
    email: str
    level: int = Field(ge=1, default=1)
    experience: int = Field(ge=0, default=0)
    favorite_team_id: Optional[str] = None
    career_teams: List[str] = []
    achievements: List[str] = []
    total_matches: int = Field(ge=0, default=0)
    total_wins: int = Field(ge=0, default=0)
    total_draws: int = Field(ge=0, default=0)
    total_losses: int = Field(ge=0, default=0)
    total_goals_scored: int = Field(ge=0, default=0)
    total_goals_conceded: int = Field(ge=0, default=0)
    preferred_formation: Formation = Formation.F_4_4_2
    control_settings: Dict[str, Any] = {}
    created_at: datetime = Field(default_factory=datetime.utcnow)
    last_login: datetime = Field(default_factory=datetime.utcnow)

class Match(BaseModel):
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    home_team_id: str
    away_team_id: str
    home_score: int = Field(ge=0, default=0)
    away_score: int = Field(ge=0, default=0)
    stadium_id: str
    game_mode: GameMode
    duration: int = Field(ge=1, le=90, default=90)
    difficulty: int = Field(ge=1, le=5, default=3)
    weather: str = "sunny"
    time_of_day: str = "day"
    completed: bool = False
    player_id: Optional[str] = None
    match_events: List[Dict[str, Any]] = []
    statistics: Dict[str, Any] = {}
    created_at: datetime = Field(default_factory=datetime.utcnow)

class Tournament(BaseModel):
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    name: str
    tournament_type: str = "cup"
    participating_teams: List[str] = []
    current_round: int = Field(ge=1, default=1)
    total_rounds: int = Field(ge=1, default=4)
    matches: List[str] = []
    winner_id: Optional[str] = None
    prize_money: int = Field(ge=0, default=1000000)
    status: str = "upcoming"
    created_at: datetime = Field(default_factory=datetime.utcnow)

class Career(BaseModel):
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    user_id: str
    current_team_id: str
    current_season: int = Field(ge=1, default=1)
    reputation: int = Field(ge=1, le=10, default=1)
    budget: int = Field(ge=0, default=5000000)
    objectives: List[Dict[str, Any]] = []
    season_stats: Dict[str, Any] = {}
    transfer_history: List[Dict[str, Any]] = []
    contract_end_date: datetime
    created_at: datetime = Field(default_factory=datetime.utcnow)

class Achievement(BaseModel):
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    name: str
    description: str
    icon: str
    category: str
    requirement: Dict[str, Any]
    reward_xp: int = Field(ge=0, default=100)
    reward_coins: int = Field(ge=0, default=1000)
    rarity: str = "common"
    unlock_condition: str
    created_at: datetime = Field(default_factory=datetime.utcnow)

class UniformKit(BaseModel):
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    team_id: str
    kit_type: str = "home"
    primary_color: str
    secondary_color: str
    accent_color: str
    pattern: str = "solid"
    sponsor: str = "Generic"
    number_font: str = "standard"
    custom_design: Dict[str, Any] = {}
    created_at: datetime = Field(default_factory=datetime.utcnow)