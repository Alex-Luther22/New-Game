import React, { useState, useEffect } from 'react';

const TutorialSystem = ({ onComplete }) => {
  const [currentStep, setCurrentStep] = useState(0);
  const [isActive, setIsActive] = useState(false);
  const [completedSteps, setCompletedSteps] = useState([]);
  const [showOverlay, setShowOverlay] = useState(false);

  const tutorialSteps = [
    {
      id: 'welcome',
      title: '¡Bienvenido a Football Master!',
      description: 'Te guiaremos paso a paso para que domines todos los controles y características del juego.',
      icon: '👋',
      target: null,
      position: 'center',
      action: 'next'
    },
    {
      id: 'controls_intro',
      title: 'Controles Básicos',
      description: 'Football Master utiliza controles táctiles intuitivos. Empecemos con lo básico.',
      icon: '🎮',
      target: '.joystick-area',
      position: 'bottom',
      action: 'highlight'
    },
    {
      id: 'joystick',
      title: 'Joystick Virtual',
      description: 'Usa el joystick virtual para mover a tu jugador. Mantén presionado y arrastra para controlar la dirección.',
      icon: '🕹️',
      target: '.joystick-area',
      position: 'right',
      action: 'practice',
      practiceTarget: 'joystick'
    },
    {
      id: 'basic_buttons',
      title: 'Botones de Acción',
      description: 'Estos botones te permiten pasar, disparar y realizar otras acciones básicas.',
      icon: '🔘',
      target: '.action-buttons',
      position: 'left',
      action: 'highlight'
    },
    {
      id: 'pass_button',
      title: 'Botón de Pase',
      description: 'Presiona este botón para pasar el balón a un compañero cercano.',
      icon: '⚽',
      target: '.pass-button',
      position: 'top',
      action: 'practice',
      practiceTarget: 'pass'
    },
    {
      id: 'shoot_button',
      title: 'Botón de Disparo',
      description: 'Usa este botón para disparar a portería cuando estés cerca del área.',
      icon: '🥅',
      target: '.shoot-button',
      position: 'top',
      action: 'practice',
      practiceTarget: 'shoot'
    },
    {
      id: 'trick_area',
      title: 'Área de Trucos',
      description: 'Esta área especial te permite realizar trucos dibujando gestos con el dedo.',
      icon: '🌟',
      target: '.trick-area',
      position: 'bottom',
      action: 'highlight'
    },
    {
      id: 'trick_practice',
      title: 'Practicar Trucos',
      description: 'Dibuja un círculo en el área de trucos para realizar una "Roulette".',
      icon: '🎭',
      target: '.trick-area',
      position: 'bottom',
      action: 'practice',
      practiceTarget: 'trick'
    },
    {
      id: 'game_controls',
      title: 'Controles de Juego',
      description: 'Estos controles te permiten cambiar jugador, ajustar cámara y pausar el juego.',
      icon: '⚙️',
      target: '.game-controls',
      position: 'bottom',
      action: 'highlight'
    },
    {
      id: 'advanced_tips',
      title: 'Consejos Avanzados',
      description: 'Combina movimientos del joystick con botones para realizar jugadas más complejas.',
      icon: '🎯',
      target: null,
      position: 'center',
      action: 'info'
    },
    {
      id: 'gamepad_support',
      title: 'Soporte para Gamepad',
      description: 'Si tienes un gamepad conectado, también puedes usarlo junto con los controles táctiles.',
      icon: '🎮',
      target: null,
      position: 'center',
      action: 'info'
    },
    {
      id: 'completion',
      title: '¡Tutorial Completado!',
      description: 'Ya conoces todos los controles básicos. ¡Ahora es hora de jugar y dominar el campo!',
      icon: '🏆',
      target: null,
      position: 'center',
      action: 'complete'
    }
  ];

  const practiceInstructions = {
    joystick: {
      title: 'Práctica del Joystick',
      instruction: 'Mantén presionado el joystick y muévelo en diferentes direcciones.',
      success: 'Has practicado el joystick correctamente',
      target: 'joystick_movement'
    },
    pass: {
      title: 'Práctica de Pase',
      instruction: 'Presiona el botón de pase para pasar el balón.',
      success: 'Pase ejecutado correctamente',
      target: 'pass_action'
    },
    shoot: {
      title: 'Práctica de Disparo',
      instruction: 'Presiona el botón de disparo para disparar a portería.',
      success: 'Disparo ejecutado correctamente',
      target: 'shoot_action'
    },
    trick: {
      title: 'Práctica de Trucos',
      instruction: 'Dibuja un círculo en el área de trucos.',
      success: 'Truco "Roulette" ejecutado correctamente',
      target: 'trick_gesture'
    }
  };

  useEffect(() => {
    // Load completed steps from localStorage
    const saved = localStorage.getItem('tutorial_completed_steps');
    if (saved) {
      setCompletedSteps(JSON.parse(saved));
    }
  }, []);

  const startTutorial = () => {
    setIsActive(true);
    setCurrentStep(0);
    setShowOverlay(true);
  };

  const nextStep = () => {
    if (currentStep < tutorialSteps.length - 1) {
      setCurrentStep(currentStep + 1);
    } else {
      completeTutorial();
    }
  };

  const previousStep = () => {
    if (currentStep > 0) {
      setCurrentStep(currentStep - 1);
    }
  };

  const skipTutorial = () => {
    setIsActive(false);
    setShowOverlay(false);
    if (onComplete) {
      onComplete();
    }
  };

  const completeTutorial = () => {
    setIsActive(false);
    setShowOverlay(false);
    const allSteps = tutorialSteps.map(step => step.id);
    setCompletedSteps(allSteps);
    localStorage.setItem('tutorial_completed_steps', JSON.stringify(allSteps));
    if (onComplete) {
      onComplete();
    }
  };

  const markStepComplete = (stepId) => {
    const newCompleted = [...completedSteps, stepId];
    setCompletedSteps(newCompleted);
    localStorage.setItem('tutorial_completed_steps', JSON.stringify(newCompleted));
  };

  const resetTutorial = () => {
    setCompletedSteps([]);
    localStorage.removeItem('tutorial_completed_steps');
    setCurrentStep(0);
  };

  const getCurrentStep = () => {
    return tutorialSteps[currentStep];
  };

  const getStepPosition = (position) => {
    const positions = {
      center: 'fixed top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2',
      top: 'fixed top-4 left-1/2 transform -translate-x-1/2',
      bottom: 'fixed bottom-4 left-1/2 transform -translate-x-1/2',
      left: 'fixed left-4 top-1/2 transform -translate-y-1/2',
      right: 'fixed right-4 top-1/2 transform -translate-y-1/2'
    };
    return positions[position] || positions.center;
  };

  const renderTutorialOverlay = () => {
    if (!showOverlay || !isActive) return null;

    const step = getCurrentStep();
    const practice = step.practiceTarget ? practiceInstructions[step.practiceTarget] : null;

    return (
      <div className="fixed inset-0 z-50 bg-black bg-opacity-50">
        {/* Tutorial Card */}
        <div className={`${getStepPosition(step.position)} w-96 max-w-sm mx-4`}>
          <div className="bg-white rounded-xl shadow-2xl p-6">
            {/* Header */}
            <div className="flex items-center justify-between mb-4">
              <div className="flex items-center space-x-3">
                <div className="text-3xl">{step.icon}</div>
                <div>
                  <h3 className="text-lg font-bold text-gray-800">{step.title}</h3>
                  <p className="text-sm text-gray-600">
                    Paso {currentStep + 1} de {tutorialSteps.length}
                  </p>
                </div>
              </div>
              <button
                onClick={skipTutorial}
                className="text-gray-400 hover:text-gray-600 transition-colors"
              >
                ✕
              </button>
            </div>

            {/* Progress Bar */}
            <div className="mb-4">
              <div className="bg-gray-200 rounded-full h-2">
                <div 
                  className="bg-green-600 h-2 rounded-full transition-all duration-300"
                  style={{ width: `${((currentStep + 1) / tutorialSteps.length) * 100}%` }}
                ></div>
              </div>
            </div>

            {/* Description */}
            <p className="text-gray-700 mb-6">{step.description}</p>

            {/* Practice Instructions */}
            {practice && (
              <div className="mb-6 p-4 bg-blue-50 rounded-lg">
                <h4 className="font-semibold text-blue-800 mb-2">{practice.title}</h4>
                <p className="text-sm text-blue-700">{practice.instruction}</p>
              </div>
            )}

            {/* Action Buttons */}
            <div className="flex space-x-3">
              {currentStep > 0 && (
                <button
                  onClick={previousStep}
                  className="flex-1 bg-gray-500 hover:bg-gray-600 text-white py-2 px-4 rounded-lg font-medium transition-colors"
                >
                  Anterior
                </button>
              )}
              
              {step.action === 'practice' ? (
                <button
                  onClick={() => {
                    markStepComplete(step.id);
                    nextStep();
                  }}
                  className="flex-1 bg-green-500 hover:bg-green-600 text-white py-2 px-4 rounded-lg font-medium transition-colors"
                >
                  Practicado ✓
                </button>
              ) : step.action === 'complete' ? (
                <button
                  onClick={completeTutorial}
                  className="flex-1 bg-green-500 hover:bg-green-600 text-white py-2 px-4 rounded-lg font-medium transition-colors"
                >
                  Finalizar
                </button>
              ) : (
                <button
                  onClick={nextStep}
                  className="flex-1 bg-blue-500 hover:bg-blue-600 text-white py-2 px-4 rounded-lg font-medium transition-colors"
                >
                  Siguiente
                </button>
              )}
            </div>

            {/* Skip Button */}
            <button
              onClick={skipTutorial}
              className="w-full mt-3 text-gray-500 hover:text-gray-700 text-sm transition-colors"
            >
              Saltar tutorial
            </button>
          </div>
        </div>

        {/* Highlight Target */}
        {step.target && (
          <div className="absolute inset-0 pointer-events-none">
            <div className="relative w-full h-full">
              {/* This would highlight the target element */}
              <div className="absolute inset-0 bg-black bg-opacity-30"></div>
            </div>
          </div>
        )}
      </div>
    );
  };

  const renderTutorialButton = () => {
    if (isActive) return null;

    return (
      <div className="fixed bottom-4 right-4 z-40">
        <button
          onClick={startTutorial}
          className="bg-green-500 hover:bg-green-600 text-white p-3 rounded-full shadow-lg transition-colors"
        >
          <div className="flex items-center space-x-2">
            <span className="text-xl">🎓</span>
            <span className="hidden sm:inline font-medium">Tutorial</span>
          </div>
        </button>
      </div>
    );
  };

  const renderTutorialMenu = () => {
    return (
      <div className="bg-white rounded-xl shadow-lg p-6">
        <h2 className="text-2xl font-bold text-gray-800 mb-4">
          🎓 Sistema de Tutorial
        </h2>
        
        <div className="mb-6">
          <p className="text-gray-600 mb-4">
            Aprende a jugar Football Master con nuestro tutorial interactivo paso a paso.
          </p>
          
          <div className="flex items-center space-x-4">
            <button
              onClick={startTutorial}
              className="bg-green-500 hover:bg-green-600 text-white px-6 py-3 rounded-lg font-semibold transition-colors"
            >
              Comenzar Tutorial
            </button>
            
            <button
              onClick={resetTutorial}
              className="bg-gray-500 hover:bg-gray-600 text-white px-6 py-3 rounded-lg font-semibold transition-colors"
            >
              Reiniciar Tutorial
            </button>
          </div>
        </div>

        {/* Tutorial Steps Overview */}
        <div className="space-y-3">
          <h3 className="text-lg font-semibold text-gray-800">Contenido del Tutorial</h3>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
            {tutorialSteps.map((step, index) => (
              <div 
                key={step.id}
                className={`flex items-center space-x-3 p-3 rounded-lg border-2 ${
                  completedSteps.includes(step.id) 
                    ? 'border-green-500 bg-green-50' 
                    : 'border-gray-200 bg-gray-50'
                }`}
              >
                <div className="text-2xl">{step.icon}</div>
                <div className="flex-1">
                  <div className="flex items-center space-x-2">
                    <h4 className="font-medium text-gray-800">{step.title}</h4>
                    {completedSteps.includes(step.id) && (
                      <span className="text-green-600 text-sm">✓</span>
                    )}
                  </div>
                  <p className="text-sm text-gray-600">{step.description}</p>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Progress Summary */}
        <div className="mt-6 p-4 bg-blue-50 rounded-lg">
          <h4 className="font-semibold text-blue-800 mb-2">Progreso del Tutorial</h4>
          <div className="flex items-center space-x-4">
            <div className="flex-1">
              <div className="bg-blue-200 rounded-full h-2">
                <div 
                  className="bg-blue-600 h-2 rounded-full transition-all duration-300"
                  style={{ width: `${(completedSteps.length / tutorialSteps.length) * 100}%` }}
                ></div>
              </div>
            </div>
            <div className="text-sm text-blue-700">
              {completedSteps.length}/{tutorialSteps.length} completado
            </div>
          </div>
        </div>
      </div>
    );
  };

  return (
    <>
      {renderTutorialOverlay()}
      {renderTutorialButton()}
      {!isActive && renderTutorialMenu()}
    </>
  );
};

export default TutorialSystem;