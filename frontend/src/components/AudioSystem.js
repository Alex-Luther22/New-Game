import React, { useState, useEffect, useRef } from 'react';

const AudioSystem = () => {
  const [isEnabled, setIsEnabled] = useState(true);
  const [currentTrack, setCurrentTrack] = useState(null);
  const [volume, setVolume] = useState(70);
  const [isPlaying, setIsPlaying] = useState(false);
  const [audioSettings, setAudioSettings] = useState({
    musicVolume: 70,
    effectsVolume: 80,
    commentaryVolume: 60,
    ambientVolume: 50,
    musicStyle: 'dynamic',
    commentaryLanguage: 'spanish',
    effectsEnabled: true,
    musicEnabled: true,
    commentaryEnabled: true,
    ambientEnabled: true
  });

  const audioRef = useRef(null);
  const effectsRef = useRef({});

  // Music tracks database
  const musicTracks = {
    menu: [
      { id: 'menu_1', name: 'Champions Rise', style: 'epic', duration: 180 },
      { id: 'menu_2', name: 'Victory Anthem', style: 'triumphant', duration: 210 },
      { id: 'menu_3', name: 'Stadium Dreams', style: 'ambient', duration: 195 }
    ],
    gameplay: [
      { id: 'game_1', name: 'Match Intensity', style: 'energetic', duration: 240 },
      { id: 'game_2', name: 'Football Fever', style: 'upbeat', duration: 220 },
      { id: 'game_3', name: 'Goal Rush', style: 'dynamic', duration: 260 }
    ],
    regional: {
      latin: [
        { id: 'latin_1', name: 'Samba Football', style: 'latin', duration: 200 },
        { id: 'latin_2', name: 'Tango Victory', style: 'latin', duration: 180 },
        { id: 'latin_3', name: 'Carnival Goal', style: 'latin', duration: 230 }
      ],
      european: [
        { id: 'euro_1', name: 'Champions League', style: 'orchestral', duration: 250 },
        { id: 'euro_2', name: 'Premier Passion', style: 'rock', duration: 210 },
        { id: 'euro_3', name: 'Nordic Thunder', style: 'electronic', duration: 240 }
      ],
      african: [
        { id: 'africa_1', name: 'Drums of Victory', style: 'tribal', duration: 190 },
        { id: 'africa_2', name: 'Savanna Sprint', style: 'energetic', duration: 200 },
        { id: 'africa_3', name: 'Ubuntu United', style: 'celebratory', duration: 220 }
      ]
    },
    situational: {
      goal: [
        { id: 'goal_1', name: 'Goal Celebration', style: 'triumphant', duration: 15 },
        { id: 'goal_2', name: 'Score Surge', style: 'epic', duration: 12 },
        { id: 'goal_3', name: 'Net Shaker', style: 'energetic', duration: 18 }
      ],
      halftime: [
        { id: 'half_1', name: 'Intermission', style: 'calm', duration: 120 },
        { id: 'half_2', name: 'Strategic Break', style: 'thoughtful', duration: 150 }
      ],
      victory: [
        { id: 'victory_1', name: 'Champions Glory', style: 'epic', duration: 60 },
        { id: 'victory_2', name: 'Victory March', style: 'triumphant', duration: 45 }
      ]
    }
  };

  // Sound effects database
  const soundEffects = {
    ball: [
      { id: 'ball_kick', name: 'Ball Kick', category: 'gameplay' },
      { id: 'ball_bounce', name: 'Ball Bounce', category: 'gameplay' },
      { id: 'ball_save', name: 'Goalkeeper Save', category: 'gameplay' },
      { id: 'ball_post', name: 'Ball Hit Post', category: 'gameplay' }
    ],
    crowd: [
      { id: 'crowd_cheer', name: 'Crowd Cheer', category: 'ambient' },
      { id: 'crowd_boo', name: 'Crowd Boo', category: 'ambient' },
      { id: 'crowd_goal', name: 'Crowd Goal Reaction', category: 'ambient' },
      { id: 'crowd_anthem', name: 'Crowd Anthem', category: 'ambient' }
    ],
    whistle: [
      { id: 'whistle_start', name: 'Match Start Whistle', category: 'referee' },
      { id: 'whistle_end', name: 'Match End Whistle', category: 'referee' },
      { id: 'whistle_foul', name: 'Foul Whistle', category: 'referee' },
      { id: 'whistle_offside', name: 'Offside Whistle', category: 'referee' }
    ],
    ui: [
      { id: 'ui_click', name: 'Button Click', category: 'interface' },
      { id: 'ui_hover', name: 'Button Hover', category: 'interface' },
      { id: 'ui_success', name: 'Success Sound', category: 'interface' },
      { id: 'ui_error', name: 'Error Sound', category: 'interface' }
    ]
  };

  // Commentary lines
  const commentaryLines = {
    spanish: {
      goal: [
        "¡GOOOOOL! ¡Qué jugada fantástica!",
        "¡Increíble! ¡El balón se cuela por la escuadra!",
        "¡Gol! ¡El estadio se viene abajo!",
        "¡Qué golazo! ¡Pura magia del fútbol!"
      ],
      save: [
        "¡Qué parada! ¡Espectacular el portero!",
        "¡Salvó el gol! ¡Manos de oro!",
        "¡Atajada increíble! ¡Evita el gol!",
        "¡Qué reflejo! ¡Salvó a su equipo!"
      ],
      miss: [
        "¡No puede ser! ¡Falla el gol!",
        "¡Por poco! ¡Rozó el palo!",
        "¡Qué oportunidad perdida!",
        "¡Increíble como falló esa!"
      ],
      start: [
        "¡Comienza el partido! ¡Que empiece el espectáculo!",
        "¡Rueda el balón! ¡Arranca la emoción!",
        "¡Pita el árbitro! ¡Comenzamos!",
        "¡Ya están en el campo! ¡Empieza la magia!"
      ]
    },
    english: {
      goal: [
        "GOAL! What a fantastic play!",
        "Incredible! The ball finds the net!",
        "Goal! The stadium erupts!",
        "What a goal! Pure football magic!"
      ],
      save: [
        "What a save! Spectacular goalkeeper!",
        "Saved the goal! Golden hands!",
        "Incredible save! Prevents the goal!",
        "What reflexes! Saved his team!"
      ],
      miss: [
        "Can't believe it! Misses the goal!",
        "So close! Hit the post!",
        "What a missed opportunity!",
        "Incredible how he missed that!"
      ],
      start: [
        "The match begins! Let the show start!",
        "The ball rolls! The excitement begins!",
        "Referee whistles! We start!",
        "They're on the field! The magic begins!"
      ]
    }
  };

  useEffect(() => {
    // Initialize audio system
    initializeAudioSystem();
    loadAudioSettings();
  }, []);

  const initializeAudioSystem = () => {
    // Create audio context for better control
    if (typeof window !== 'undefined' && window.AudioContext) {
      const audioContext = new (window.AudioContext || window.webkitAudioContext)();
      // Initialize audio nodes for different channels
    }
  };

  const loadAudioSettings = () => {
    const saved = localStorage.getItem('audio_settings');
    if (saved) {
      setAudioSettings(JSON.parse(saved));
    }
  };

  const saveAudioSettings = (settings) => {
    localStorage.setItem('audio_settings', JSON.stringify(settings));
    setAudioSettings(settings);
  };

  const playMusic = (category, trackId = null) => {
    if (!audioSettings.musicEnabled) return;

    let tracks = [];
    if (category === 'menu') {
      tracks = musicTracks.menu;
    } else if (category === 'gameplay') {
      tracks = musicTracks.gameplay;
    } else if (category.includes('regional.')) {
      const region = category.split('.')[1];
      tracks = musicTracks.regional[region] || [];
    } else if (category.includes('situational.')) {
      const situation = category.split('.')[1];
      tracks = musicTracks.situational[situation] || [];
    }

    const track = trackId ? tracks.find(t => t.id === trackId) : tracks[Math.floor(Math.random() * tracks.length)];
    
    if (track) {
      setCurrentTrack(track);
      setIsPlaying(true);
      // In a real implementation, you would load and play the actual audio file
      console.log(`Playing: ${track.name} (${track.style})`);
    }
  };

  const playEffect = (effectId) => {
    if (!audioSettings.effectsEnabled) return;

    // Find the effect in the database
    const allEffects = Object.values(soundEffects).flat();
    const effect = allEffects.find(e => e.id === effectId);
    
    if (effect) {
      // In a real implementation, you would play the actual sound effect
      console.log(`Playing effect: ${effect.name}`);
    }
  };

  const playCommentary = (situation) => {
    if (!audioSettings.commentaryEnabled) return;

    const language = audioSettings.commentaryLanguage;
    const lines = commentaryLines[language]?.[situation] || [];
    
    if (lines.length > 0) {
      const line = lines[Math.floor(Math.random() * lines.length)];
      // In a real implementation, you would play the actual commentary audio
      console.log(`Commentary: ${line}`);
    }
  };

  const stopMusic = () => {
    setIsPlaying(false);
    setCurrentTrack(null);
  };

  const toggleMusic = () => {
    if (isPlaying) {
      stopMusic();
    } else {
      playMusic('menu');
    }
  };

  const changeMusicVolume = (newVolume) => {
    const settings = { ...audioSettings, musicVolume: newVolume };
    saveAudioSettings(settings);
    setVolume(newVolume);
  };

  const renderVolumeSlider = (label, value, onChange, icon) => (
    <div className="space-y-2">
      <div className="flex items-center justify-between">
        <label className="text-sm font-medium text-gray-700 flex items-center space-x-2">
          <span className="text-lg">{icon}</span>
          <span>{label}</span>
        </label>
        <span className="text-sm text-gray-600">{value}%</span>
      </div>
      <div className="flex items-center space-x-3">
        <input
          type="range"
          min="0"
          max="100"
          value={value}
          onChange={(e) => onChange(parseInt(e.target.value))}
          className="flex-1 h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer"
        />
        <button
          onClick={() => onChange(value === 0 ? 70 : 0)}
          className={`w-8 h-8 rounded-full flex items-center justify-center text-xs ${
            value === 0 ? 'bg-red-500 text-white' : 'bg-gray-200 text-gray-600'
          }`}
        >
          {value === 0 ? '🔇' : '🔊'}
        </button>
      </div>
    </div>
  );

  const renderAudioSettings = () => (
    <div className="bg-white rounded-xl shadow-lg p-6">
      <h2 className="text-2xl font-bold text-gray-800 mb-6">
        🎵 Sistema de Audio
      </h2>

      {/* Master Controls */}
      <div className="mb-8">
        <h3 className="text-lg font-semibold text-gray-800 mb-4">Control Principal</h3>
        <div className="flex items-center space-x-4 mb-4">
          <button
            onClick={() => setIsEnabled(!isEnabled)}
            className={`px-4 py-2 rounded-lg font-medium transition-colors ${
              isEnabled ? 'bg-green-500 text-white' : 'bg-red-500 text-white'
            }`}
          >
            {isEnabled ? '🔊 Audio Activado' : '🔇 Audio Desactivado'}
          </button>
          
          <button
            onClick={toggleMusic}
            className={`px-4 py-2 rounded-lg font-medium transition-colors ${
              isPlaying ? 'bg-blue-500 text-white' : 'bg-gray-500 text-white'
            }`}
          >
            {isPlaying ? '⏸️ Pausar' : '▶️ Reproducir'}
          </button>
        </div>

        {/* Current Track Display */}
        {currentTrack && (
          <div className="bg-blue-50 p-4 rounded-lg">
            <div className="flex items-center justify-between">
              <div>
                <h4 className="font-medium text-blue-800">{currentTrack.name}</h4>
                <p className="text-sm text-blue-600">
                  Estilo: {currentTrack.style} | Duración: {Math.floor(currentTrack.duration / 60)}:{(currentTrack.duration % 60).toString().padStart(2, '0')}
                </p>
              </div>
              <div className="text-2xl">
                {isPlaying ? '🎵' : '⏸️'}
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Volume Controls */}
      <div className="mb-8">
        <h3 className="text-lg font-semibold text-gray-800 mb-4">Controles de Volumen</h3>
        <div className="space-y-6">
          {renderVolumeSlider(
            'Música',
            audioSettings.musicVolume,
            (value) => saveAudioSettings({ ...audioSettings, musicVolume: value }),
            '🎵'
          )}
          {renderVolumeSlider(
            'Efectos de Sonido',
            audioSettings.effectsVolume,
            (value) => saveAudioSettings({ ...audioSettings, effectsVolume: value }),
            '🔊'
          )}
          {renderVolumeSlider(
            'Comentarios',
            audioSettings.commentaryVolume,
            (value) => saveAudioSettings({ ...audioSettings, commentaryVolume: value }),
            '🎤'
          )}
          {renderVolumeSlider(
            'Ambiente',
            audioSettings.ambientVolume,
            (value) => saveAudioSettings({ ...audioSettings, ambientVolume: value }),
            '🌍'
          )}
        </div>
      </div>

      {/* Audio Preferences */}
      <div className="mb-8">
        <h3 className="text-lg font-semibold text-gray-800 mb-4">Preferencias de Audio</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              🎭 Estilo de Música
            </label>
            <select
              value={audioSettings.musicStyle}
              onChange={(e) => saveAudioSettings({ ...audioSettings, musicStyle: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
            >
              <option value="dynamic">Dinámico (Según situación)</option>
              <option value="energetic">Energético</option>
              <option value="ambient">Ambiente</option>
              <option value="epic">Épico</option>
              <option value="regional">Regional</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              🗣️ Idioma de Comentarios
            </label>
            <select
              value={audioSettings.commentaryLanguage}
              onChange={(e) => saveAudioSettings({ ...audioSettings, commentaryLanguage: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
            >
              <option value="spanish">Español</option>
              <option value="english">English</option>
            </select>
          </div>
        </div>
      </div>

      {/* Feature Toggles */}
      <div className="mb-8">
        <h3 className="text-lg font-semibold text-gray-800 mb-4">Activar/Desactivar</h3>
        <div className="grid grid-cols-2 gap-4">
          {[
            { key: 'musicEnabled', label: 'Música', icon: '🎵' },
            { key: 'effectsEnabled', label: 'Efectos', icon: '🔊' },
            { key: 'commentaryEnabled', label: 'Comentarios', icon: '🎤' },
            { key: 'ambientEnabled', label: 'Ambiente', icon: '🌍' }
          ].map((feature) => (
            <label key={feature.key} className="flex items-center space-x-3 p-3 border border-gray-200 rounded-lg cursor-pointer hover:bg-gray-50">
              <input
                type="checkbox"
                checked={audioSettings[feature.key]}
                onChange={(e) => saveAudioSettings({ ...audioSettings, [feature.key]: e.target.checked })}
                className="w-5 h-5 text-green-600 rounded focus:ring-green-500"
              />
              <span className="text-lg">{feature.icon}</span>
              <span className="font-medium text-gray-700">{feature.label}</span>
            </label>
          ))}
        </div>
      </div>

      {/* Test Audio */}
      <div className="mb-8">
        <h3 className="text-lg font-semibold text-gray-800 mb-4">Probar Audio</h3>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
          <button
            onClick={() => playEffect('ball_kick')}
            className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-lg font-medium transition-colors"
          >
            ⚽ Patada
          </button>
          <button
            onClick={() => playEffect('crowd_cheer')}
            className="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded-lg font-medium transition-colors"
          >
            👏 Ovación
          </button>
          <button
            onClick={() => playEffect('whistle_start')}
            className="bg-yellow-500 hover:bg-yellow-600 text-white px-4 py-2 rounded-lg font-medium transition-colors"
          >
            🎵 Silbato
          </button>
          <button
            onClick={() => playCommentary('goal')}
            className="bg-purple-500 hover:bg-purple-600 text-white px-4 py-2 rounded-lg font-medium transition-colors"
          >
            🎤 Gol
          </button>
        </div>
      </div>

      {/* Music Library */}
      <div className="mb-8">
        <h3 className="text-lg font-semibold text-gray-800 mb-4">Biblioteca Musical</h3>
        <div className="space-y-4">
          {Object.entries(musicTracks).map(([category, tracks]) => (
            <div key={category} className="border border-gray-200 rounded-lg p-4">
              <h4 className="font-medium text-gray-800 mb-3 capitalize">{category}</h4>
              <div className="space-y-2">
                {Array.isArray(tracks) ? tracks.map((track) => (
                  <div key={track.id} className="flex items-center justify-between p-2 bg-gray-50 rounded">
                    <div>
                      <span className="font-medium text-gray-700">{track.name}</span>
                      <span className="text-sm text-gray-500 ml-2">({track.style})</span>
                    </div>
                    <button
                      onClick={() => playMusic(category, track.id)}
                      className="bg-green-500 hover:bg-green-600 text-white px-3 py-1 rounded text-sm transition-colors"
                    >
                      ▶️
                    </button>
                  </div>
                )) : (
                  <div className="text-sm text-gray-500">
                    {Object.keys(tracks).length} subcategorías disponibles
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );

  return (
    <div className="max-w-4xl mx-auto p-6">
      {renderAudioSettings()}
    </div>
  );
};

export default AudioSystem;