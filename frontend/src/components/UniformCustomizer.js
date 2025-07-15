import React, { useState, useEffect } from 'react';
import axios from 'axios';

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API = `${BACKEND_URL}/api`;

const UniformCustomizer = ({ teamId }) => {
  const [team, setTeam] = useState(null);
  const [uniforms, setUniforms] = useState([]);
  const [selectedUniform, setSelectedUniform] = useState(null);
  const [loading, setLoading] = useState(true);
  const [currentView, setCurrentView] = useState('gallery');
  const [customization, setCustomization] = useState({
    kit_type: 'home',
    primary_color: '#FF0000',
    secondary_color: '#FFFFFF',
    accent_color: '#000000',
    pattern: 'solid',
    sponsor: 'Generic',
    number_font: 'standard',
    custom_design: {}
  });

  const patterns = [
    { id: 'solid', name: 'Sólido', preview: 'bg-gradient-to-b' },
    { id: 'stripes_vertical', name: 'Rayas Verticales', preview: 'bg-gradient-to-r' },
    { id: 'stripes_horizontal', name: 'Rayas Horizontales', preview: 'bg-gradient-to-b' },
    { id: 'diagonal', name: 'Diagonal', preview: 'bg-gradient-to-br' },
    { id: 'checkered', name: 'Cuadros', preview: 'bg-gradient-to-tr' },
    { id: 'gradient', name: 'Degradado', preview: 'bg-gradient-to-t' },
    { id: 'panels', name: 'Paneles', preview: 'bg-gradient-to-l' },
    { id: 'diamond', name: 'Diamante', preview: 'bg-gradient-to-tl' }
  ];

  const numberFonts = [
    { id: 'standard', name: 'Estándar', style: 'font-sans' },
    { id: 'bold', name: 'Negrita', style: 'font-bold' },
    { id: 'italic', name: 'Cursiva', style: 'font-serif italic' },
    { id: 'modern', name: 'Moderno', style: 'font-mono' },
    { id: 'classic', name: 'Clásico', style: 'font-serif' }
  ];

  const sponsors = [
    'Generic', 'SportsTech', 'PlayMax', 'EliteGear', 'ProSport', 
    'Champion', 'Victory', 'Athletic', 'PowerPlay', 'GameTime'
  ];

  useEffect(() => {
    if (teamId) {
      fetchTeamData();
      fetchUniforms();
    }
  }, [teamId]);

  const fetchTeamData = async () => {
    try {
      const response = await axios.get(`${API}/teams/${teamId}`);
      setTeam(response.data);
      setCustomization(prev => ({
        ...prev,
        primary_color: response.data.primary_color,
        secondary_color: response.data.secondary_color
      }));
    } catch (error) {
      console.error('Error fetching team data:', error);
    }
  };

  const fetchUniforms = async () => {
    try {
      const response = await axios.get(`${API}/teams/${teamId}/uniforms`);
      setUniforms(response.data);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching uniforms:', error);
      setLoading(false);
    }
  };

  const createUniform = async () => {
    try {
      const uniformData = {
        ...customization,
        team_id: teamId
      };
      
      await axios.post(`${API}/teams/${teamId}/uniforms`, uniformData);
      fetchUniforms();
      setCurrentView('gallery');
    } catch (error) {
      console.error('Error creating uniform:', error);
    }
  };

  const renderColorPicker = (label, value, onChange) => (
    <div className="space-y-2">
      <label className="block text-sm font-medium text-gray-700">
        {label}
      </label>
      <div className="flex items-center space-x-3">
        <input
          type="color"
          value={value}
          onChange={(e) => onChange(e.target.value)}
          className="w-16 h-10 rounded-lg border border-gray-300 cursor-pointer"
        />
        <input
          type="text"
          value={value}
          onChange={(e) => onChange(e.target.value)}
          className="flex-1 px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
          placeholder="#FF0000"
        />
        <div
          className="w-10 h-10 rounded-lg border border-gray-300"
          style={{ backgroundColor: value }}
        ></div>
      </div>
    </div>
  );

  const renderUniformPreview = () => (
    <div className="bg-white rounded-xl shadow-lg p-6">
      <h3 className="text-xl font-bold text-gray-800 mb-4">👕 Vista Previa</h3>
      
      {/* Jersey Preview */}
      <div className="flex justify-center mb-6">
        <div className="relative">
          {/* Jersey Body */}
          <div
            className={`w-32 h-40 rounded-t-3xl ${patterns.find(p => p.id === customization.pattern)?.preview || 'bg-gradient-to-b'}`}
            style={{
              background: customization.pattern === 'solid' 
                ? customization.primary_color
                : `linear-gradient(to bottom, ${customization.primary_color}, ${customization.secondary_color})`
            }}
          >
            {/* Jersey Details */}
            <div className="absolute top-4 left-1/2 transform -translate-x-1/2">
              <div className="text-white text-xs font-bold text-center">
                {customization.sponsor}
              </div>
            </div>
            
            {/* Player Number */}
            <div className="absolute bottom-4 left-1/2 transform -translate-x-1/2">
              <div 
                className={`text-4xl font-bold ${numberFonts.find(f => f.id === customization.number_font)?.style || ''}`}
                style={{ color: customization.accent_color }}
              >
                10
              </div>
            </div>
          </div>
          
          {/* Jersey Arms */}
          <div
            className="absolute top-8 -left-6 w-12 h-20 rounded-full"
            style={{ backgroundColor: customization.secondary_color }}
          ></div>
          <div
            className="absolute top-8 -right-6 w-12 h-20 rounded-full"
            style={{ backgroundColor: customization.secondary_color }}
          ></div>
        </div>
      </div>
      
      {/* Shorts Preview */}
      <div className="flex justify-center mb-4">
        <div
          className="w-24 h-16 rounded-b-lg"
          style={{ backgroundColor: customization.secondary_color }}
        ></div>
      </div>
      
      {/* Socks Preview */}
      <div className="flex justify-center">
        <div
          className="w-16 h-12 rounded-t-lg"
          style={{ backgroundColor: customization.primary_color }}
        ></div>
      </div>
      
      {/* Kit Info */}
      <div className="mt-6 p-4 bg-gray-50 rounded-lg">
        <h4 className="font-semibold text-gray-800 mb-2">Información del Kit</h4>
        <div className="space-y-1 text-sm text-gray-600">
          <p><strong>Tipo:</strong> {customization.kit_type}</p>
          <p><strong>Patrón:</strong> {patterns.find(p => p.id === customization.pattern)?.name}</p>
          <p><strong>Patrocinador:</strong> {customization.sponsor}</p>
          <p><strong>Fuente números:</strong> {numberFonts.find(f => f.id === customization.number_font)?.name}</p>
        </div>
      </div>
    </div>
  );

  const renderCustomizer = () => (
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
      {/* Customization Controls */}
      <div className="lg:col-span-2 space-y-6">
        {/* Kit Type */}
        <div className="bg-white rounded-xl shadow-lg p-6">
          <h3 className="text-xl font-bold text-gray-800 mb-4">🏠 Tipo de Kit</h3>
          <div className="grid grid-cols-3 gap-4">
            {['home', 'away', 'third'].map((type) => (
              <button
                key={type}
                onClick={() => setCustomization(prev => ({ ...prev, kit_type: type }))}
                className={`p-4 rounded-lg border-2 transition-all ${
                  customization.kit_type === type
                    ? 'border-green-500 bg-green-50'
                    : 'border-gray-200 hover:border-green-300'
                }`}
              >
                <div className="text-2xl mb-2">
                  {type === 'home' ? '🏠' : type === 'away' ? '✈️' : '🎯'}
                </div>
                <div className="font-medium capitalize">{type}</div>
              </button>
            ))}
          </div>
        </div>

        {/* Colors */}
        <div className="bg-white rounded-xl shadow-lg p-6">
          <h3 className="text-xl font-bold text-gray-800 mb-4">🎨 Colores</h3>
          <div className="space-y-4">
            {renderColorPicker(
              'Color Principal',
              customization.primary_color,
              (color) => setCustomization(prev => ({ ...prev, primary_color: color }))
            )}
            {renderColorPicker(
              'Color Secundario',
              customization.secondary_color,
              (color) => setCustomization(prev => ({ ...prev, secondary_color: color }))
            )}
            {renderColorPicker(
              'Color de Acento',
              customization.accent_color,
              (color) => setCustomization(prev => ({ ...prev, accent_color: color }))
            )}
          </div>
        </div>

        {/* Pattern */}
        <div className="bg-white rounded-xl shadow-lg p-6">
          <h3 className="text-xl font-bold text-gray-800 mb-4">🎭 Patrón</h3>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
            {patterns.map((pattern) => (
              <button
                key={pattern.id}
                onClick={() => setCustomization(prev => ({ ...prev, pattern: pattern.id }))}
                className={`p-4 rounded-lg border-2 transition-all ${
                  customization.pattern === pattern.id
                    ? 'border-green-500 bg-green-50'
                    : 'border-gray-200 hover:border-green-300'
                }`}
              >
                <div 
                  className={`w-full h-16 rounded-lg mb-2 ${pattern.preview}`}
                  style={{
                    background: pattern.id === 'solid' 
                      ? customization.primary_color
                      : `linear-gradient(to bottom, ${customization.primary_color}, ${customization.secondary_color})`
                  }}
                ></div>
                <div className="text-sm font-medium text-gray-700">
                  {pattern.name}
                </div>
              </button>
            ))}
          </div>
        </div>

        {/* Sponsor */}
        <div className="bg-white rounded-xl shadow-lg p-6">
          <h3 className="text-xl font-bold text-gray-800 mb-4">🏢 Patrocinador</h3>
          <select
            value={customization.sponsor}
            onChange={(e) => setCustomization(prev => ({ ...prev, sponsor: e.target.value }))}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
          >
            {sponsors.map((sponsor) => (
              <option key={sponsor} value={sponsor}>
                {sponsor}
              </option>
            ))}
          </select>
        </div>

        {/* Number Font */}
        <div className="bg-white rounded-xl shadow-lg p-6">
          <h3 className="text-xl font-bold text-gray-800 mb-4">🔢 Fuente de Números</h3>
          <div className="grid grid-cols-2 md:grid-cols-3 gap-3">
            {numberFonts.map((font) => (
              <button
                key={font.id}
                onClick={() => setCustomization(prev => ({ ...prev, number_font: font.id }))}
                className={`p-4 rounded-lg border-2 transition-all ${
                  customization.number_font === font.id
                    ? 'border-green-500 bg-green-50'
                    : 'border-gray-200 hover:border-green-300'
                }`}
              >
                <div className={`text-2xl mb-2 ${font.style}`}>10</div>
                <div className="text-sm font-medium text-gray-700">
                  {font.name}
                </div>
              </button>
            ))}
          </div>
        </div>

        {/* Save Button */}
        <div className="flex justify-center">
          <button
            onClick={createUniform}
            className="bg-green-500 hover:bg-green-600 text-white px-8 py-3 rounded-lg font-semibold text-lg transition-colors shadow-lg"
          >
            💾 Guardar Uniforme
          </button>
        </div>
      </div>

      {/* Preview */}
      <div className="lg:col-span-1">
        {renderUniformPreview()}
      </div>
    </div>
  );

  const renderGallery = () => (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h2 className="text-3xl font-bold text-gray-800">
          👕 Uniformes del Equipo
        </h2>
        <button
          onClick={() => setCurrentView('customizer')}
          className="bg-green-500 hover:bg-green-600 text-white px-6 py-3 rounded-lg font-semibold transition-colors"
        >
          ➕ Crear Nuevo Uniforme
        </button>
      </div>

      {team && (
        <div className="bg-white rounded-xl shadow-lg p-6">
          <h3 className="text-xl font-bold text-gray-800 mb-4">
            Uniformes de {team.name}
          </h3>
          <div className="flex items-center space-x-4 mb-4">
            <div className="flex space-x-2">
              <div 
                className="w-8 h-8 rounded-full border-2 border-white shadow-sm"
                style={{ backgroundColor: team.primary_color }}
              ></div>
              <div 
                className="w-8 h-8 rounded-full border-2 border-white shadow-sm"
                style={{ backgroundColor: team.secondary_color }}
              ></div>
            </div>
            <span className="text-gray-600">Colores oficiales del equipo</span>
          </div>
        </div>
      )}

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {uniforms.map((uniform) => (
          <div key={uniform.id} className="bg-white rounded-xl shadow-lg p-6">
            <div className="flex justify-center mb-4">
              <div
                className="w-24 h-30 rounded-t-2xl"
                style={{ 
                  backgroundColor: uniform.primary_color,
                  backgroundImage: uniform.pattern !== 'solid' 
                    ? `linear-gradient(to bottom, ${uniform.primary_color}, ${uniform.secondary_color})`
                    : 'none'
                }}
              >
                <div className="text-center pt-2">
                  <div className="text-white text-xs font-bold">
                    {uniform.sponsor}
                  </div>
                  <div 
                    className="text-2xl font-bold mt-4"
                    style={{ color: uniform.accent_color }}
                  >
                    10
                  </div>
                </div>
              </div>
            </div>
            
            <div className="text-center">
              <h4 className="font-semibold text-gray-800 capitalize mb-2">
                Kit {uniform.kit_type}
              </h4>
              <div className="text-sm text-gray-600 space-y-1">
                <p>Patrón: {uniform.pattern}</p>
                <p>Patrocinador: {uniform.sponsor}</p>
                <p>Fuente: {uniform.number_font}</p>
              </div>
            </div>
          </div>
        ))}
      </div>

      {uniforms.length === 0 && (
        <div className="text-center py-12">
          <div className="text-6xl mb-4">👕</div>
          <h3 className="text-xl font-semibold text-gray-800 mb-2">
            No hay uniformes creados
          </h3>
          <p className="text-gray-600 mb-4">
            Crea tu primer uniforme personalizado para el equipo
          </p>
          <button
            onClick={() => setCurrentView('customizer')}
            className="bg-green-500 hover:bg-green-600 text-white px-6 py-3 rounded-lg font-semibold transition-colors"
          >
            ➕ Crear Primer Uniforme
          </button>
        </div>
      )}
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
      {currentView === 'gallery' ? (
        renderGallery()
      ) : (
        <div className="space-y-6">
          <div className="flex justify-between items-center">
            <h2 className="text-3xl font-bold text-gray-800">
              🎨 Personalizador de Uniformes
            </h2>
            <button
              onClick={() => setCurrentView('gallery')}
              className="bg-gray-500 hover:bg-gray-600 text-white px-6 py-3 rounded-lg font-semibold transition-colors"
            >
              ← Volver a Galería
            </button>
          </div>
          {renderCustomizer()}
        </div>
      )}
    </div>
  );
};

export default UniformCustomizer;