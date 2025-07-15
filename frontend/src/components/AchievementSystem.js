import React, { useState, useEffect } from 'react';
import axios from 'axios';

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API = `${BACKEND_URL}/api`;

const AchievementSystem = ({ userId }) => {
  const [achievements, setAchievements] = useState([]);
  const [userAchievements, setUserAchievements] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedCategory, setSelectedCategory] = useState('all');
  const [selectedRarity, setSelectedRarity] = useState('all');
  const [userStats, setUserStats] = useState(null);

  useEffect(() => {
    fetchAchievements();
    if (userId) {
      fetchUserAchievements();
      fetchUserStats();
    }
  }, [userId]);

  const fetchAchievements = async () => {
    try {
      const response = await axios.get(`${API}/achievements`);
      setAchievements(response.data);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching achievements:', error);
      setLoading(false);
    }
  };

  const fetchUserAchievements = async () => {
    try {
      const response = await axios.get(`${API}/users/${userId}/achievements`);
      setUserAchievements(response.data);
    } catch (error) {
      console.error('Error fetching user achievements:', error);
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

  const unlockAchievement = async (achievementId) => {
    try {
      await axios.post(`${API}/users/${userId}/achievements/${achievementId}`);
      fetchUserAchievements();
    } catch (error) {
      console.error('Error unlocking achievement:', error);
    }
  };

  const getRarityColor = (rarity) => {
    const colors = {
      'common': 'bg-gray-100 text-gray-800 border-gray-300',
      'rare': 'bg-blue-100 text-blue-800 border-blue-300',
      'epic': 'bg-purple-100 text-purple-800 border-purple-300',
      'legendary': 'bg-yellow-100 text-yellow-800 border-yellow-300'
    };
    return colors[rarity] || colors.common;
  };

  const getRarityIcon = (rarity) => {
    const icons = {
      'common': '⚪',
      'rare': '🔵',
      'epic': '🟣',
      'legendary': '🟡'
    };
    return icons[rarity] || '⚪';
  };

  const getCategoryIcon = (category) => {
    const icons = {
      'Scoring': '⚽',
      'Defending': '🛡️',
      'Streak': '🔥',
      'Season': '🏆',
      'Career': '👔',
      'Trophies': '🏅',
      'Transfers': '🔄',
      'Skills': '🌟',
      'Exploration': '🌍',
      'Progress': '📈'
    };
    return icons[category] || '🎯';
  };

  const isAchievementUnlocked = (achievementId) => {
    return userAchievements.some(ua => ua.id === achievementId);
  };

  const checkAchievementProgress = (achievement) => {
    if (!userStats) return { progress: 0, total: 1, percentage: 0 };
    
    const requirement = achievement.requirement;
    const stats = userStats.match_statistics;
    
    let progress = 0;
    let total = 1;
    
    // Check different achievement types
    if (requirement.goals_scored) {
      progress = stats.goals_scored || 0;
      total = requirement.goals_scored;
    } else if (requirement.goals_in_match) {
      progress = 0; // This would need to be tracked per match
      total = requirement.goals_in_match;
    } else if (requirement.clean_sheets) {
      progress = 0; // This would need to be tracked
      total = requirement.clean_sheets;
    } else if (requirement.unbeaten_streak) {
      progress = 0; // This would need to be tracked
      total = requirement.unbeaten_streak;
    } else if (requirement.level) {
      progress = userStats.level || 0;
      total = requirement.level;
    } else if (requirement.trophies_won) {
      progress = 0; // This would need to be tracked
      total = requirement.trophies_won;
    }
    
    const percentage = total > 0 ? Math.min((progress / total) * 100, 100) : 0;
    
    return { progress, total, percentage };
  };

  const getFilteredAchievements = () => {
    let filtered = achievements;
    
    if (selectedCategory !== 'all') {
      filtered = filtered.filter(achievement => achievement.category === selectedCategory);
    }
    
    if (selectedRarity !== 'all') {
      filtered = filtered.filter(achievement => achievement.rarity === selectedRarity);
    }
    
    return filtered;
  };

  const getCategories = () => {
    const categories = [...new Set(achievements.map(a => a.category))];
    return categories.sort();
  };

  const getRarities = () => {
    const rarities = [...new Set(achievements.map(a => a.rarity))];
    return rarities.sort();
  };

  const getAchievementStats = () => {
    const total = achievements.length;
    const unlocked = userAchievements.length;
    const percentage = total > 0 ? Math.round((unlocked / total) * 100) : 0;
    
    const byRarity = {
      common: { total: 0, unlocked: 0 },
      rare: { total: 0, unlocked: 0 },
      epic: { total: 0, unlocked: 0 },
      legendary: { total: 0, unlocked: 0 }
    };
    
    achievements.forEach(achievement => {
      if (byRarity[achievement.rarity]) {
        byRarity[achievement.rarity].total++;
        if (isAchievementUnlocked(achievement.id)) {
          byRarity[achievement.rarity].unlocked++;
        }
      }
    });
    
    return { total, unlocked, percentage, byRarity };
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-green-600"></div>
      </div>
    );
  }

  const stats = getAchievementStats();
  const filteredAchievements = getFilteredAchievements();

  return (
    <div className="max-w-7xl mx-auto p-6">
      {/* Header */}
      <div className="text-center mb-8">
        <h1 className="text-4xl font-bold text-gray-800 mb-4">
          🏆 Sistema de Logros
        </h1>
        <p className="text-lg text-gray-600">
          Desbloquea logros y demuestra tu maestría en Football Master
        </p>
      </div>

      {/* Stats Overview */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div className="bg-white rounded-xl shadow-lg p-6">
          <div className="flex items-center justify-between">
            <div>
              <h3 className="text-lg font-semibold text-gray-800">Total Logros</h3>
              <p className="text-3xl font-bold text-green-600">
                {stats.unlocked}/{stats.total}
              </p>
            </div>
            <div className="text-4xl">🎯</div>
          </div>
          <div className="mt-4">
            <div className="bg-gray-200 rounded-full h-2">
              <div 
                className="bg-green-600 h-2 rounded-full transition-all duration-300"
                style={{ width: `${stats.percentage}%` }}
              ></div>
            </div>
            <p className="text-sm text-gray-600 mt-1">{stats.percentage}% completado</p>
          </div>
        </div>

        {Object.entries(stats.byRarity).map(([rarity, data]) => (
          <div key={rarity} className="bg-white rounded-xl shadow-lg p-6">
            <div className="flex items-center justify-between">
              <div>
                <h3 className="text-lg font-semibold text-gray-800 capitalize">{rarity}</h3>
                <p className="text-2xl font-bold text-blue-600">
                  {data.unlocked}/{data.total}
                </p>
              </div>
              <div className="text-4xl">{getRarityIcon(rarity)}</div>
            </div>
            <div className="mt-4">
              <div className="bg-gray-200 rounded-full h-2">
                <div 
                  className="bg-blue-600 h-2 rounded-full transition-all duration-300"
                  style={{ width: `${data.total > 0 ? (data.unlocked / data.total) * 100 : 0}%` }}
                ></div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Filters */}
      <div className="bg-white rounded-xl shadow-lg p-6 mb-8">
        <div className="flex flex-wrap gap-4 items-center">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Categoría
            </label>
            <select
              value={selectedCategory}
              onChange={(e) => setSelectedCategory(e.target.value)}
              className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
            >
              <option value="all">Todas las categorías</option>
              {getCategories().map(category => (
                <option key={category} value={category}>
                  {getCategoryIcon(category)} {category}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Rareza
            </label>
            <select
              value={selectedRarity}
              onChange={(e) => setSelectedRarity(e.target.value)}
              className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
            >
              <option value="all">Todas las rarezas</option>
              {getRarities().map(rarity => (
                <option key={rarity} value={rarity}>
                  {getRarityIcon(rarity)} {rarity}
                </option>
              ))}
            </select>
          </div>

          <div className="flex-1 text-right">
            <p className="text-sm text-gray-600">
              Mostrando {filteredAchievements.length} de {achievements.length} logros
            </p>
          </div>
        </div>
      </div>

      {/* Achievements Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {filteredAchievements.map((achievement) => {
          const isUnlocked = isAchievementUnlocked(achievement.id);
          const progress = checkAchievementProgress(achievement);
          
          return (
            <div
              key={achievement.id}
              className={`bg-white rounded-xl shadow-lg overflow-hidden transition-all duration-300 hover:shadow-xl ${
                isUnlocked ? 'ring-2 ring-green-500' : ''
              }`}
            >
              {/* Achievement Header */}
              <div className={`p-4 ${isUnlocked ? 'bg-green-50' : 'bg-gray-50'}`}>
                <div className="flex items-center justify-between mb-2">
                  <div className="text-3xl">{achievement.icon}</div>
                  <div className={`px-3 py-1 rounded-full text-xs font-medium border ${getRarityColor(achievement.rarity)}`}>
                    {getRarityIcon(achievement.rarity)} {achievement.rarity}
                  </div>
                </div>
                <h3 className="text-lg font-bold text-gray-800 mb-1">
                  {achievement.name}
                </h3>
                <p className="text-sm text-gray-600">
                  {achievement.description}
                </p>
              </div>

              {/* Achievement Body */}
              <div className="p-4">
                <div className="flex items-center justify-between mb-3">
                  <div className="flex items-center space-x-2">
                    <span className="text-sm font-medium text-gray-700">
                      {getCategoryIcon(achievement.category)} {achievement.category}
                    </span>
                  </div>
                  <div className="text-right">
                    <div className="text-sm text-gray-600">
                      {isUnlocked ? '✅ Desbloqueado' : `${progress.progress}/${progress.total}`}
                    </div>
                  </div>
                </div>

                {/* Progress Bar */}
                {!isUnlocked && (
                  <div className="mb-4">
                    <div className="bg-gray-200 rounded-full h-2">
                      <div 
                        className="bg-green-600 h-2 rounded-full transition-all duration-300"
                        style={{ width: `${progress.percentage}%` }}
                      ></div>
                    </div>
                    <p className="text-xs text-gray-600 mt-1">
                      {Math.round(progress.percentage)}% completado
                    </p>
                  </div>
                )}

                {/* Rewards */}
                <div className="space-y-2">
                  <div className="flex items-center justify-between text-sm">
                    <span className="text-gray-600">Recompensa XP:</span>
                    <span className="font-medium text-blue-600">
                      +{achievement.reward_xp.toLocaleString()} XP
                    </span>
                  </div>
                  <div className="flex items-center justify-between text-sm">
                    <span className="text-gray-600">Recompensa Monedas:</span>
                    <span className="font-medium text-yellow-600">
                      +{achievement.reward_coins.toLocaleString()} 🪙
                    </span>
                  </div>
                </div>

                {/* Unlock Button */}
                {!isUnlocked && progress.percentage >= 100 && (
                  <button
                    onClick={() => unlockAchievement(achievement.id)}
                    className="w-full mt-4 bg-green-500 hover:bg-green-600 text-white py-2 px-4 rounded-lg font-semibold transition-colors"
                  >
                    🔓 Desbloquear Logro
                  </button>
                )}

                {/* Unlocked Badge */}
                {isUnlocked && (
                  <div className="mt-4 bg-green-100 text-green-800 py-2 px-4 rounded-lg text-center font-semibold">
                    ✅ Logro Desbloqueado
                  </div>
                )}
              </div>
            </div>
          );
        })}
      </div>

      {filteredAchievements.length === 0 && (
        <div className="text-center py-12">
          <div className="text-6xl mb-4">🏆</div>
          <h3 className="text-xl font-semibold text-gray-800 mb-2">
            No se encontraron logros
          </h3>
          <p className="text-gray-600">
            Prueba con diferentes filtros para encontrar logros
          </p>
        </div>
      )}
    </div>
  );
};

export default AchievementSystem;