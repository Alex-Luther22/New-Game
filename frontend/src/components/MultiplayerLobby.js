import React, { useState, useEffect } from 'react';
import axios from 'axios';

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API = `${BACKEND_URL}/api`;

const MultiplayerLobby = ({ userId }) => {
  const [activeMatches, setActiveMatches] = useState([]);
  const [availableOpponents, setAvailableOpponents] = useState([]);
  const [selectedOpponent, setSelectedOpponent] = useState(null);
  const [matchSettings, setMatchSettings] = useState({
    duration: 90,
    difficulty: 3,
    stadium_id: '',
    weather: 'sunny',
    time_of_day: 'day'
  });
  const [loading, setLoading] = useState(true);
  const [currentView, setCurrentView] = useState('lobby');
  const [matchQueue, setMatchQueue] = useState([]);
  const [userStats, setUserStats] = useState(null);

  useEffect(() => {
    fetchActiveMatches();
    fetchAvailableOpponents();
    fetchUserStats();
    // Simulate real-time updates
    const interval = setInterval(() => {
      fetchActiveMatches();
      fetchAvailableOpponents();
    }, 5000);
    
    return () => clearInterval(interval);
  }, []);

  const fetchActiveMatches = async () => {
    try {
      const response = await axios.get(`${API}/users/${userId}/matches`);
      const onlineMatches = response.data.filter(match => 
        match.game_mode === 'online' && !match.completed
      );
      setActiveMatches(onlineMatches);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching active matches:', error);
      setLoading(false);
    }
  };

  const fetchAvailableOpponents = async () => {
    try {
      // Simulate fetching online players
      const mockOpponents = [
        {
          id: 'player1',
          username: 'FutbolMaster',
          level: 25,
          rating: 1850,
          country: 'Spain',
          wins: 145,
          losses: 67,
          favorite_team: 'Madrid White',
          online: true,
          last_seen: new Date().toISOString()
        },
        {
          id: 'player2',
          username: 'GoalKeeper99',
          level: 18,
          rating: 1650,
          country: 'Brazil',
          wins: 89,
          losses: 45,
          favorite_team: 'Flamengo',
          online: true,
          last_seen: new Date().toISOString()
        },
        {
          id: 'player3',
          username: 'TacticalGenius',
          level: 32,
          rating: 2100,
          country: 'Germany',
          wins: 234,
          losses: 89,
          favorite_team: 'Bayern Munich',
          online: true,
          last_seen: new Date().toISOString()
        },
        {
          id: 'player4',
          username: 'SpeedDemon',
          level: 22,
          rating: 1750,
          country: 'England',
          wins: 167,
          losses: 78,
          favorite_team: 'Manchester Blue',
          online: true,
          last_seen: new Date().toISOString()
        },
        {
          id: 'player5',
          username: 'DefenseWall',
          level: 28,
          rating: 1950,
          country: 'Italy',
          wins: 198,
          losses: 56,
          favorite_team: 'Juventus FC',
          online: true,
          last_seen: new Date().toISOString()
        }
      ];
      setAvailableOpponents(mockOpponents);
    } catch (error) {
      console.error('Error fetching opponents:', error);
    }
  };

  const fetchUserStats = async () => {
    try {
      const response = await axios.get(`${API}/users/${userId}/statistics`);
      setUserStats(response.data);
    } catch (error) {
      console.error('Error fetching user stats:', error);
    }
  };

  const createOnlineMatch = async (opponentId) => {
    try {
      const matchData = {
        home_team_id: userStats?.profile?.favorite_team_id || 'default_team',
        away_team_id: 'opponent_team',
        stadium_id: matchSettings.stadium_id || 'default_stadium',
        game_mode: 'online',
        duration: matchSettings.duration,
        difficulty: matchSettings.difficulty,
        weather: matchSettings.weather,
        time_of_day: matchSettings.time_of_day,
        player_id: userId,
        opponent_id: opponentId,
        match_events: [],
        statistics: {}
      };

      const response = await axios.post(`${API}/matches`, matchData);
      if (response.data.match_id) {
        fetchActiveMatches();
        setCurrentView('match');
      }
    } catch (error) {
      console.error('Error creating match:', error);
    }
  };

  const joinMatchQueue = () => {
    setMatchQueue(prev => [...prev, {
      id: Date.now(),
      userId,
      timestamp: new Date().toISOString(),
      settings: matchSettings
    }]);
    setCurrentView('queue');
  };

  const leaveMatchQueue = () => {
    setMatchQueue(prev => prev.filter(item => item.userId !== userId));
    setCurrentView('lobby');
  };

  const getRatingColor = (rating) => {
    if (rating >= 2000) return 'text-purple-600';
    if (rating >= 1800) return 'text-yellow-600';
    if (rating >= 1600) return 'text-green-600';
    if (rating >= 1400) return 'text-blue-600';
    return 'text-gray-600';
  };

  const getRatingBadge = (rating) => {
    if (rating >= 2000) return 'bg-purple-100 text-purple-800';
    if (rating >= 1800) return 'bg-yellow-100 text-yellow-800';
    if (rating >= 1600) return 'bg-green-100 text-green-800';
    if (rating >= 1400) return 'bg-blue-100 text-blue-800';
    return 'bg-gray-100 text-gray-800';
  };

  const getCountryFlag = (country) => {
    const flags = {
      'Spain': '🇪🇸',
      'Brazil': '🇧🇷',
      'Germany': '🇩🇪',
      'England': '🏴󠁧󠁢󠁥󠁮󠁧󠁿',
      'Italy': '🇮🇹',
      'France': '🇫🇷',
      'Argentina': '🇦🇷',
      'Netherlands': '🇳🇱',
      'Portugal': '🇵🇹',
      'Belgium': '🇧🇪'
    };
    return flags[country] || '🌍';
  };

  const renderLobby = () => (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-green-500 to-blue-500 rounded-xl p-6 text-white">
        <h2 className="text-3xl font-bold mb-2">🌐 Multijugador Online</h2>
        <p className="text-lg opacity-90">Compite contra jugadores de todo el mundo</p>
        <div className="mt-4 flex items-center space-x-6">
          <div className="text-center">
            <div className="text-2xl font-bold">{availableOpponents.length}</div>
            <div className="text-sm opacity-90">Jugadores Online</div>
          </div>
          <div className="text-center">
            <div className="text-2xl font-bold">{activeMatches.length}</div>
            <div className="text-sm opacity-90">Partidas Activas</div>
          </div>
        </div>
      </div>

      {/* Quick Match */}
      <div className="bg-white rounded-xl shadow-lg p-6">
        <h3 className="text-xl font-bold text-gray-800 mb-4">⚡ Partida Rápida</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Duración (minutos)
            </label>
            <select
              value={matchSettings.duration}
              onChange={(e) => setMatchSettings(prev => ({ ...prev, duration: parseInt(e.target.value) }))}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
            >
              <option value={45}>45 minutos</option>
              <option value={90}>90 minutos</option>
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Dificultad
            </label>
            <select
              value={matchSettings.difficulty}
              onChange={(e) => setMatchSettings(prev => ({ ...prev, difficulty: parseInt(e.target.value) }))}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
            >
              <option value={1}>Muy Fácil</option>
              <option value={2}>Fácil</option>
              <option value={3}>Normal</option>
              <option value={4}>Difícil</option>
              <option value={5}>Muy Difícil</option>
            </select>
          </div>
        </div>
        <button
          onClick={joinMatchQueue}
          className="w-full bg-green-500 hover:bg-green-600 text-white py-3 px-6 rounded-lg font-semibold transition-colors"
        >
          🎯 Buscar Partida
        </button>
      </div>

      {/* Available Opponents */}
      <div className="bg-white rounded-xl shadow-lg p-6">
        <h3 className="text-xl font-bold text-gray-800 mb-4">👥 Jugadores Disponibles</h3>
        <div className="space-y-4">
          {availableOpponents.map((opponent) => (
            <div
              key={opponent.id}
              className="flex items-center justify-between p-4 border border-gray-200 rounded-lg hover:border-green-300 transition-colors"
            >
              <div className="flex items-center space-x-4">
                <div className="w-12 h-12 bg-gradient-to-r from-green-400 to-blue-400 rounded-full flex items-center justify-center text-white font-bold text-lg">
                  {opponent.username.charAt(0).toUpperCase()}
                </div>
                <div>
                  <div className="flex items-center space-x-2">
                    <h4 className="font-semibold text-gray-800">{opponent.username}</h4>
                    <span className={`px-2 py-1 rounded-full text-xs font-medium ${getRatingBadge(opponent.rating)}`}>
                      {opponent.rating}
                    </span>
                    <span className="text-sm">{getCountryFlag(opponent.country)}</span>
                  </div>
                  <div className="text-sm text-gray-600">
                    Nivel {opponent.level} | {opponent.wins}W-{opponent.losses}L | {opponent.favorite_team}
                  </div>
                  <div className="flex items-center space-x-1 text-xs text-green-600">
                    <div className="w-2 h-2 bg-green-500 rounded-full"></div>
                    <span>Online</span>
                  </div>
                </div>
              </div>
              <div className="flex space-x-2">
                <button
                  onClick={() => setSelectedOpponent(opponent)}
                  className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-lg font-medium transition-colors"
                >
                  👤 Ver Perfil
                </button>
                <button
                  onClick={() => createOnlineMatch(opponent.id)}
                  className="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded-lg font-medium transition-colors"
                >
                  ⚽ Desafiar
                </button>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Active Matches */}
      {activeMatches.length > 0 && (
        <div className="bg-white rounded-xl shadow-lg p-6">
          <h3 className="text-xl font-bold text-gray-800 mb-4">🔥 Partidas Activas</h3>
          <div className="space-y-4">
            {activeMatches.map((match) => (
              <div
                key={match.id}
                className="flex items-center justify-between p-4 border border-gray-200 rounded-lg"
              >
                <div>
                  <h4 className="font-semibold text-gray-800">
                    Partida vs Oponente
                  </h4>
                  <div className="text-sm text-gray-600">
                    {match.duration} min | {match.weather} | {match.time_of_day}
                  </div>
                </div>
                <div className="flex space-x-2">
                  <span className="px-3 py-1 bg-yellow-100 text-yellow-800 rounded-full text-sm font-medium">
                    En Progreso
                  </span>
                  <button className="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded-lg font-medium transition-colors">
                    Continuar
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );

  const renderQueue = () => (
    <div className="space-y-6">
      <div className="bg-white rounded-xl shadow-lg p-8 text-center">
        <div className="text-6xl mb-4">🔍</div>
        <h2 className="text-2xl font-bold text-gray-800 mb-4">
          Buscando Oponente...
        </h2>
        <p className="text-gray-600 mb-6">
          Estamos buscando un jugador con tu nivel para una partida equilibrada
        </p>
        
        {/* Animated Loading */}
        <div className="flex justify-center mb-6">
          <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-green-600"></div>
        </div>
        
        {/* Queue Status */}
        <div className="bg-gray-50 rounded-lg p-4 mb-6">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <div className="text-sm text-gray-600">Tiempo en cola</div>
              <div className="text-lg font-semibold text-gray-800">0:32</div>
            </div>
            <div>
              <div className="text-sm text-gray-600">Jugadores en cola</div>
              <div className="text-lg font-semibold text-gray-800">{matchQueue.length}</div>
            </div>
          </div>
        </div>
        
        {/* Settings Summary */}
        <div className="bg-blue-50 rounded-lg p-4 mb-6">
          <h3 className="font-semibold text-blue-800 mb-2">Configuración de Partida</h3>
          <div className="text-sm text-blue-700">
            <p>Duración: {matchSettings.duration} minutos</p>
            <p>Dificultad: {matchSettings.difficulty}/5</p>
            <p>Clima: {matchSettings.weather}</p>
          </div>
        </div>
        
        <button
          onClick={leaveMatchQueue}
          className="bg-red-500 hover:bg-red-600 text-white px-6 py-3 rounded-lg font-semibold transition-colors"
        >
          Cancelar Búsqueda
        </button>
      </div>
    </div>
  );

  const renderOpponentProfile = () => (
    <div className="bg-white rounded-xl shadow-lg p-6">
      <button
        onClick={() => setSelectedOpponent(null)}
        className="mb-4 text-gray-600 hover:text-gray-800 transition-colors"
      >
        ← Volver al lobby
      </button>
      
      <div className="text-center mb-6">
        <div className="w-20 h-20 bg-gradient-to-r from-green-400 to-blue-400 rounded-full flex items-center justify-center text-white font-bold text-2xl mx-auto mb-4">
          {selectedOpponent?.username.charAt(0).toUpperCase()}
        </div>
        <h2 className="text-2xl font-bold text-gray-800">{selectedOpponent?.username}</h2>
        <p className="text-gray-600 flex items-center justify-center space-x-2">
          <span>{getCountryFlag(selectedOpponent?.country)}</span>
          <span>{selectedOpponent?.country}</span>
        </p>
      </div>
      
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="space-y-4">
          <div className="bg-gray-50 p-4 rounded-lg">
            <h3 className="font-semibold text-gray-800 mb-2">Estadísticas</h3>
            <div className="space-y-2">
              <div className="flex justify-between">
                <span className="text-gray-600">Nivel:</span>
                <span className="font-medium">{selectedOpponent?.level}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Rating:</span>
                <span className={`font-medium ${getRatingColor(selectedOpponent?.rating)}`}>
                  {selectedOpponent?.rating}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Victorias:</span>
                <span className="font-medium text-green-600">{selectedOpponent?.wins}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Derrotas:</span>
                <span className="font-medium text-red-600">{selectedOpponent?.losses}</span>
              </div>
            </div>
          </div>
        </div>
        
        <div className="space-y-4">
          <div className="bg-gray-50 p-4 rounded-lg">
            <h3 className="font-semibold text-gray-800 mb-2">Información</h3>
            <div className="space-y-2">
              <div className="flex justify-between">
                <span className="text-gray-600">Equipo favorito:</span>
                <span className="font-medium">{selectedOpponent?.favorite_team}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Estado:</span>
                <span className="flex items-center space-x-1 text-green-600">
                  <div className="w-2 h-2 bg-green-500 rounded-full"></div>
                  <span>Online</span>
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <div className="mt-6 text-center">
        <button
          onClick={() => createOnlineMatch(selectedOpponent.id)}
          className="bg-green-500 hover:bg-green-600 text-white px-8 py-3 rounded-lg font-semibold transition-colors"
        >
          ⚽ Desafiar a {selectedOpponent?.username}
        </button>
      </div>
    </div>
  );

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-green-600"></div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto p-6">
      {currentView === 'lobby' && renderLobby()}
      {currentView === 'queue' && renderQueue()}
      {selectedOpponent && renderOpponentProfile()}
    </div>
  );
};

export default MultiplayerLobby;