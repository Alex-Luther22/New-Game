import React, { useState, useRef, useEffect } from 'react';

const TouchControlsDemo = () => {
  const canvasRef = useRef(null);
  const [ballPosition, setBallPosition] = useState({ x: 300, y: 300 });
  const [joystickPosition, setJoystickPosition] = useState({ x: 0, y: 0 });
  const [isJoystickActive, setIsJoystickActive] = useState(false);
  const [activeButton, setActiveButton] = useState('');
  const [trickGesture, setTrickGesture] = useState([]);
  const [detectedTrick, setDetectedTrick] = useState('');
  const [isTrickAreaActive, setIsTrickAreaActive] = useState(false);
  const [playerPosition, setPlayerPosition] = useState({ x: 250, y: 300 });
  const [isMoving, setIsMoving] = useState(false);
  const [isCustomizationMode, setIsCustomizationMode] = useState(false);
  const [controlSettings, setControlSettings] = useState({
    joystickPos: { x: 80, y: 320 },
    trickAreaPos: { x: 320, y: 80 },
    buttonSize: 35,
    controlOpacity: 0.8
  });
  const [gameFeatures, setGameFeatures] = useState({
    ballFollow: true,
    autoPlayerSwitch: true,
    playerNames: false,
    gameSpeed: 1.0
  });
  const [currentPlayer, setCurrentPlayer] = useState('10');
  const [ballSpeed, setBallSpeed] = useState(0);
  const [gamepadConnected, setGamepadConnected] = useState(false);

  // Configuración de controles expandida
  const joystickArea = { 
    x: controlSettings.joystickPos.x, 
    y: controlSettings.joystickPos.y, 
    radius: 75 
  };
  
  // Área de trucos expandida en superior derecha
  const trickArea = { 
    x: controlSettings.trickAreaPos.x, 
    y: controlSettings.trickAreaPos.y, 
    width: 250, 
    height: 150 
  };
  
  // Botones organizados mejor
  const buttons = {
    pass: { x: 450, y: 320, radius: controlSettings.buttonSize, label: 'PASE', color: '#4CAF50' },
    shoot: { x: 520, y: 250, radius: controlSettings.buttonSize, label: 'DISPARO', color: '#F44336' },
    throughPass: { x: 380, y: 250, radius: controlSettings.buttonSize - 5, label: 'PASE PROF', color: '#2196F3' },
    cross: { x: 520, y: 390, radius: controlSettings.buttonSize - 5, label: 'CENTRO', color: '#FF9800' },
    sprint: { x: 380, y: 390, radius: controlSettings.buttonSize - 5, label: 'SPRINT', color: '#9C27B0' },
    tackle: { x: 450, y: 220, radius: controlSettings.buttonSize - 5, label: 'TACKLE', color: '#607D8B' },
    slideTackle: { x: 450, y: 420, radius: controlSettings.buttonSize - 5, label: 'BARRIDA', color: '#795548' },
    callForBall: { x: 550, y: 320, radius: controlSettings.buttonSize - 5, label: 'PEDIR', color: '#00BCD4' }
  };

  // Controles de juego
  const gameControls = {
    changePlayer: { x: 100, y: 50, width: 80, height: 30, label: 'CAMBIAR', active: false },
    ballCamera: { x: 190, y: 50, width: 80, height: 30, label: 'CÁMARA', active: gameFeatures.ballFollow },
    substitution: { x: 280, y: 50, width: 80, height: 30, label: 'CAMBIO', active: false },
    pause: { x: 500, y: 50, width: 60, height: 30, label: 'PAUSA', active: false }
  };

  // 12 trucos expandidos
  const trickPatterns = {
    'Step-over': { difficulty: 1, description: 'Dibuja zigzag', pattern: 'zigzag', unlocked: true },
    'Roulette': { difficulty: 2, description: 'Dibuja círculo completo', pattern: 'circle', unlocked: true },
    'Elastico': { difficulty: 3, description: 'Dibuja L (derecha-abajo)', pattern: 'L', unlocked: true },
    'Nutmeg': { difficulty: 2, description: 'Dibuja línea vertical', pattern: 'vertical', unlocked: true },
    'Rainbow Flick': { difficulty: 4, description: 'Dibuja arco hacia arriba', pattern: 'arc', unlocked: false },
    'Rabona': { difficulty: 4, description: 'Dibuja curva externa', pattern: 'curve', unlocked: false },
    'Heel Flick': { difficulty: 3, description: 'Dibuja hacia atrás', pattern: 'backward', unlocked: false },
    'Scorpion': { difficulty: 5, description: 'Dibuja S compleja', pattern: 'S', unlocked: false },
    'Maradona Turn': { difficulty: 4, description: 'Dibuja medio círculo', pattern: 'halfCircle', unlocked: false },
    'Cruyff Turn': { difficulty: 3, description: 'Dibuja V invertida', pattern: 'V', unlocked: false },
    'Bicycle Kick': { difficulty: 5, description: 'Dibuja X', pattern: 'X', unlocked: false },
    'Fake Shot': { difficulty: 2, description: 'Dibuja línea corta', pattern: 'shortLine', unlocked: true }
  };

  useEffect(() => {
    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    
    canvas.width = 600;
    canvas.height = 400;
    
    // Limpiar canvas
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    
    // Dibujar campo
    drawField(ctx);
    
    // Dibujar jugador
    drawPlayer(ctx, playerPosition.x, playerPosition.y);
    
    // Dibujar balón
    drawBall(ctx, ballPosition.x, ballPosition.y);
    
    // Dibujar controles
    drawControls(ctx);
    
    // Dibujar gesto de truco
    if (trickGesture.length > 1) {
      drawTrickGesture(ctx, trickGesture);
    }
    
  }, [ballPosition, playerPosition, joystickPosition, isJoystickActive, activeButton, trickGesture, isTrickAreaActive]);

  const drawField = (ctx) => {
    // Fondo verde
    ctx.fillStyle = '#2E7D32';
    ctx.fillRect(0, 0, 600, 400);
    
    // Líneas del campo
    ctx.strokeStyle = '#FFFFFF';
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.moveTo(300, 0);
    ctx.lineTo(300, 400);
    ctx.stroke();
    
    // Círculo central
    ctx.beginPath();
    ctx.arc(300, 200, 40, 0, 2 * Math.PI);
    ctx.stroke();
    
    // Áreas
    ctx.strokeRect(30, 120, 80, 160);
    ctx.strokeRect(490, 120, 80, 160);
    
    // Porterías
    ctx.strokeRect(0, 170, 30, 60);
    ctx.strokeRect(570, 170, 30, 60);
  };

  const drawPlayer = (ctx, x, y) => {
    // Sombra del jugador
    ctx.fillStyle = 'rgba(0, 0, 0, 0.3)';
    ctx.beginPath();
    ctx.ellipse(x + 1, y + 1, 12, 8, 0, 0, 2 * Math.PI);
    ctx.fill();
    
    // Jugador
    ctx.fillStyle = isMoving ? '#1565C0' : '#1976D2';
    ctx.beginPath();
    ctx.arc(x, y, 10, 0, 2 * Math.PI);
    ctx.fill();
    
    // Número del jugador
    ctx.fillStyle = '#FFFFFF';
    ctx.font = '12px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('10', x, y + 4);
  };

  const drawBall = (ctx, x, y) => {
    // Sombra del balón
    ctx.fillStyle = 'rgba(0, 0, 0, 0.3)';
    ctx.beginPath();
    ctx.ellipse(x + 1, y + 1, 8, 6, 0, 0, 2 * Math.PI);
    ctx.fill();
    
    // Balón
    ctx.fillStyle = '#FFFFFF';
    ctx.beginPath();
    ctx.arc(x, y, 6, 0, 2 * Math.PI);
    ctx.fill();
    
    // Líneas del balón
    ctx.strokeStyle = '#000000';
    ctx.lineWidth = 1;
    ctx.beginPath();
    ctx.moveTo(x - 4, y - 4);
    ctx.lineTo(x + 4, y + 4);
    ctx.moveTo(x - 4, y + 4);
    ctx.lineTo(x + 4, y - 4);
    ctx.stroke();
  };

  const drawControls = (ctx) => {
    // Área de joystick
    ctx.fillStyle = 'rgba(255, 255, 255, 0.1)';
    ctx.beginPath();
    ctx.arc(joystickArea.x, joystickArea.y, joystickArea.radius, 0, 2 * Math.PI);
    ctx.fill();
    
    // Joystick background
    ctx.fillStyle = isJoystickActive ? 'rgba(0, 150, 255, 0.3)' : 'rgba(255, 255, 255, 0.2)';
    ctx.beginPath();
    ctx.arc(joystickArea.x, joystickArea.y, 40, 0, 2 * Math.PI);
    ctx.fill();
    
    // Joystick handle
    const handleX = joystickArea.x + joystickPosition.x * 30;
    const handleY = joystickArea.y + joystickPosition.y * 30;
    ctx.fillStyle = isJoystickActive ? '#00BCD4' : '#FFFFFF';
    ctx.beginPath();
    ctx.arc(handleX, handleY, 15, 0, 2 * Math.PI);
    ctx.fill();
    
    // Botones de acción
    Object.entries(buttons).forEach(([key, button]) => {
      const isActive = activeButton === key;
      
      // Botón
      ctx.fillStyle = isActive ? '#4CAF50' : 'rgba(255, 255, 255, 0.3)';
      ctx.beginPath();
      ctx.arc(button.x, button.y, button.radius, 0, 2 * Math.PI);
      ctx.fill();
      
      // Borde
      ctx.strokeStyle = isActive ? '#2E7D32' : '#FFFFFF';
      ctx.lineWidth = 2;
      ctx.stroke();
      
      // Texto
      ctx.fillStyle = isActive ? '#FFFFFF' : '#000000';
      ctx.font = '10px Arial';
      ctx.textAlign = 'center';
      ctx.fillText(button.label, button.x, button.y + 3);
    });
    
    // Área de trucos
    ctx.fillStyle = isTrickAreaActive ? 'rgba(255, 255, 0, 0.3)' : 'rgba(255, 255, 255, 0.1)';
    ctx.fillRect(trickArea.x, trickArea.y, trickArea.width, trickArea.height);
    
    // Borde del área de trucos
    ctx.strokeStyle = isTrickAreaActive ? '#FFD700' : '#FFFFFF';
    ctx.lineWidth = 2;
    ctx.strokeRect(trickArea.x, trickArea.y, trickArea.width, trickArea.height);
    
    // Texto del área de trucos
    ctx.fillStyle = '#FFFFFF';
    ctx.font = '12px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('ÁREA DE TRUCOS', trickArea.x + trickArea.width / 2, trickArea.y + 20);
    ctx.fillText('Dibuja gestos aquí', trickArea.x + trickArea.width / 2, trickArea.y + 35);
    
    // Labels de controles
    ctx.fillStyle = '#FFFFFF';
    ctx.font = '12px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('JOYSTICK', joystickArea.x, joystickArea.y + 60);
    ctx.fillText('MOVER/APUNTAR', joystickArea.x, joystickArea.y + 75);
  };

  const drawTrickGesture = (ctx, gesture) => {
    if (gesture.length < 2) return;
    
    ctx.strokeStyle = '#FFD700';
    ctx.lineWidth = 3;
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';
    
    ctx.beginPath();
    ctx.moveTo(gesture[0].x, gesture[0].y);
    
    for (let i = 1; i < gesture.length; i++) {
      ctx.lineTo(gesture[i].x, gesture[i].y);
    }
    
    ctx.stroke();
  };

  const handleMouseDown = (e) => {
    const rect = canvasRef.current.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;
    
    // Verificar joystick
    const joystickDistance = Math.sqrt(
      Math.pow(x - joystickArea.x, 2) + Math.pow(y - joystickArea.y, 2)
    );
    
    if (joystickDistance <= joystickArea.radius) {
      setIsJoystickActive(true);
      updateJoystick(x, y);
      return;
    }
    
    // Verificar botones
    for (const [key, button] of Object.entries(buttons)) {
      const buttonDistance = Math.sqrt(
        Math.pow(x - button.x, 2) + Math.pow(y - button.y, 2)
      );
      
      if (buttonDistance <= button.radius) {
        setActiveButton(key);
        executeAction(key);
        return;
      }
    }
    
    // Verificar área de trucos
    if (x >= trickArea.x && x <= trickArea.x + trickArea.width &&
        y >= trickArea.y && y <= trickArea.y + trickArea.height) {
      setIsTrickAreaActive(true);
      setTrickGesture([{ x, y }]);
      return;
    }
  };

  const handleMouseMove = (e) => {
    const rect = canvasRef.current.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;
    
    if (isJoystickActive) {
      updateJoystick(x, y);
    }
    
    if (isTrickAreaActive) {
      setTrickGesture(prev => [...prev, { x, y }]);
    }
  };

  const handleMouseUp = () => {
    if (isJoystickActive) {
      setIsJoystickActive(false);
      setJoystickPosition({ x: 0, y: 0 });
      setIsMoving(false);
    }
    
    if (activeButton) {
      setActiveButton('');
    }
    
    if (isTrickAreaActive) {
      setIsTrickAreaActive(false);
      if (trickGesture.length > 5) {
        const trick = detectTrick(trickGesture);
        setDetectedTrick(trick);
        if (trick !== 'Ninguno') {
          executeTrick(trick);
        }
      }
      setTimeout(() => {
        setTrickGesture([]);
        setDetectedTrick('');
      }, 2000);
    }
  };

  const updateJoystick = (x, y) => {
    const deltaX = x - joystickArea.x;
    const deltaY = y - joystickArea.y;
    const distance = Math.sqrt(deltaX * deltaX + deltaY * deltaY);
    
    if (distance > 30) {
      const normalizedX = deltaX / distance;
      const normalizedY = deltaY / distance;
      setJoystickPosition({ x: normalizedX, y: normalizedY });
    } else {
      setJoystickPosition({ x: deltaX / 30, y: deltaY / 30 });
    }
    
    // Mover jugador
    if (Math.abs(deltaX) > 5 || Math.abs(deltaY) > 5) {
      setIsMoving(true);
      const newPlayerPos = {
        x: Math.max(20, Math.min(580, playerPosition.x + deltaX * 0.1)),
        y: Math.max(20, Math.min(380, playerPosition.y + deltaY * 0.1))
      };
      setPlayerPosition(newPlayerPos);
    }
  };

  const executeAction = (action) => {
    switch (action) {
      case 'pass':
        executePass();
        break;
      case 'shoot':
        executeShoot();
        break;
      case 'sprint':
        executeSprint();
        break;
      case 'tackle':
        executeTackle();
        break;
    }
  };

  const executePass = () => {
    const direction = joystickPosition.x > 0 ? 1 : -1;
    const newBallPos = {
      x: Math.max(20, Math.min(580, ballPosition.x + direction * 50)),
      y: ballPosition.y
    };
    
    animateBall(newBallPos);
  };

  const executeShoot = () => {
    const direction = joystickPosition.x > 0 ? 1 : -1;
    const newBallPos = {
      x: Math.max(20, Math.min(580, ballPosition.x + direction * 100)),
      y: Math.max(20, Math.min(380, ballPosition.y + joystickPosition.y * 30))
    };
    
    animateBall(newBallPos);
  };

  const executeSprint = () => {
    setIsMoving(true);
    setTimeout(() => setIsMoving(false), 1000);
  };

  const executeTackle = () => {
    // Animación de tackle
    const originalPos = { ...playerPosition };
    setPlayerPosition({ x: originalPos.x + 20, y: originalPos.y });
    setTimeout(() => setPlayerPosition(originalPos), 300);
  };

  const animateBall = (targetPos) => {
    const startPos = { ...ballPosition };
    const steps = 20;
    let currentStep = 0;
    
    const animate = () => {
      if (currentStep < steps) {
        const progress = currentStep / steps;
        setBallPosition({
          x: startPos.x + (targetPos.x - startPos.x) * progress,
          y: startPos.y + (targetPos.y - startPos.y) * progress
        });
        currentStep++;
        setTimeout(animate, 50);
      }
    };
    
    animate();
  };

  const detectTrick = (gesture) => {
    if (gesture.length < 5) return 'Ninguno';
    
    if (isCircularGesture(gesture)) return 'Roulette';
    if (isLShapeGesture(gesture)) return 'Elastico';
    if (isZigzagGesture(gesture)) return 'Step-over';
    if (isVerticalLineGesture(gesture)) return 'Nutmeg';
    
    return 'Ninguno';
  };

  const isCircularGesture = (gesture) => {
    const center = calculateCenter(gesture);
    const avgRadius = calculateAverageRadius(gesture, center);
    let circularPoints = 0;
    
    for (let point of gesture) {
      const distance = Math.sqrt(Math.pow(point.x - center.x, 2) + Math.pow(point.y - center.y, 2));
      if (Math.abs(distance - avgRadius) < avgRadius * 0.3) {
        circularPoints++;
      }
    }
    
    return circularPoints / gesture.length > 0.6;
  };

  const isLShapeGesture = (gesture) => {
    if (gesture.length < 6) return false;
    const midPoint = Math.floor(gesture.length / 2);
    const firstHalf = gesture.slice(0, midPoint);
    const secondHalf = gesture.slice(midPoint);
    
    return isHorizontalGesture(firstHalf) && isVerticalGesture(secondHalf);
  };

  const isZigzagGesture = (gesture) => {
    let directionChanges = 0;
    for (let i = 2; i < gesture.length; i++) {
      const angle1 = Math.atan2(gesture[i-1].y - gesture[i-2].y, gesture[i-1].x - gesture[i-2].x);
      const angle2 = Math.atan2(gesture[i].y - gesture[i-1].y, gesture[i].x - gesture[i-1].x);
      if (Math.abs(angle1 - angle2) > Math.PI / 3) {
        directionChanges++;
      }
    }
    return directionChanges >= 2;
  };

  const isVerticalLineGesture = (gesture) => {
    const start = gesture[0];
    const end = gesture[gesture.length - 1];
    const horizontal = Math.abs(end.x - start.x);
    const vertical = Math.abs(end.y - start.y);
    return vertical > horizontal * 2;
  };

  const isHorizontalGesture = (gesture) => {
    const start = gesture[0];
    const end = gesture[gesture.length - 1];
    const horizontal = Math.abs(end.x - start.x);
    const vertical = Math.abs(end.y - start.y);
    return horizontal > vertical * 1.5;
  };

  const isVerticalGesture = (gesture) => {
    const start = gesture[0];
    const end = gesture[gesture.length - 1];
    const horizontal = Math.abs(end.x - start.x);
    const vertical = Math.abs(end.y - start.y);
    return vertical > horizontal * 1.5;
  };

  const calculateCenter = (points) => {
    const sum = points.reduce((acc, point) => ({
      x: acc.x + point.x,
      y: acc.y + point.y
    }), { x: 0, y: 0 });
    return { x: sum.x / points.length, y: sum.y / points.length };
  };

  const calculateAverageRadius = (points, center) => {
    const totalRadius = points.reduce((acc, point) => {
      return acc + Math.sqrt(Math.pow(point.x - center.x, 2) + Math.pow(point.y - center.y, 2));
    }, 0);
    return totalRadius / points.length;
  };

  const executeTrick = (trick) => {
    // Animación del truco
    const originalPos = { ...playerPosition };
    let animationSteps = [];
    
    switch (trick) {
      case 'Roulette':
        animationSteps = [
          { x: originalPos.x + 5, y: originalPos.y },
          { x: originalPos.x, y: originalPos.y - 5 },
          { x: originalPos.x - 5, y: originalPos.y },
          { x: originalPos.x, y: originalPos.y + 5 },
          originalPos
        ];
        break;
      case 'Elastico':
        animationSteps = [
          { x: originalPos.x + 10, y: originalPos.y },
          { x: originalPos.x, y: originalPos.y - 10 },
          originalPos
        ];
        break;
      case 'Step-over':
        animationSteps = [
          { x: originalPos.x + 8, y: originalPos.y },
          { x: originalPos.x - 8, y: originalPos.y },
          originalPos
        ];
        break;
      case 'Nutmeg':
        animationSteps = [
          { x: originalPos.x, y: originalPos.y + 15 },
          originalPos
        ];
        break;
    }
    
    // Ejecutar animación
    let stepIndex = 0;
    const animateStep = () => {
      if (stepIndex < animationSteps.length) {
        setPlayerPosition(animationSteps[stepIndex]);
        stepIndex++;
        setTimeout(animateStep, 150);
      }
    };
    
    animateStep();
  };

  const resetDemo = () => {
    setBallPosition({ x: 300, y: 300 });
    setPlayerPosition({ x: 250, y: 300 });
    setJoystickPosition({ x: 0, y: 0 });
    setIsJoystickActive(false);
    setActiveButton('');
    setTrickGesture([]);
    setDetectedTrick('');
    setIsTrickAreaActive(false);
    setIsMoving(false);
  };

  return (
    <div className="max-w-6xl mx-auto p-6 bg-gradient-to-br from-green-50 to-blue-50 min-h-screen">
      <div className="bg-white rounded-xl shadow-2xl p-8">
        <h1 className="text-4xl font-bold text-center mb-8 text-gray-800">
          ⚽ Football Master - Controles Híbridos
        </h1>
        
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Campo de Juego */}
          <div className="lg:col-span-2">
            <div className="bg-gradient-to-br from-green-600 to-green-700 p-4 rounded-lg">
              <h2 className="text-xl font-semibold text-white mb-4 text-center">
                🎮 Demo Interactiva - Controles Híbridos
              </h2>
              
              <div className="bg-white p-4 rounded-lg">
                <canvas
                  ref={canvasRef}
                  onMouseDown={handleMouseDown}
                  onMouseMove={handleMouseMove}
                  onMouseUp={handleMouseUp}
                  onTouchStart={handleMouseDown}
                  onTouchMove={handleMouseMove}
                  onTouchEnd={handleMouseUp}
                  className="border-2 border-green-800 rounded-lg cursor-crosshair w-full"
                  style={{ maxWidth: '600px', height: '400px' }}
                />
              </div>
              
              <div className="mt-4 text-center">
                <button
                  onClick={resetDemo}
                  className="bg-blue-500 hover:bg-blue-600 text-white px-6 py-2 rounded-lg font-semibold transition-colors"
                >
                  🔄 Reiniciar Demo
                </button>
              </div>
            </div>
            
            {/* Información en tiempo real */}
            <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="bg-blue-100 p-4 rounded-lg">
                <h3 className="font-semibold text-blue-800 mb-2">🎯 Estado Actual</h3>
                <p className="text-blue-700">
                  <strong>Jugador:</strong> {isMoving ? 'Moviéndose' : 'Parado'}
                </p>
                <p className="text-blue-700">
                  <strong>Joystick:</strong> {isJoystickActive ? 'Activo' : 'Inactivo'}
                </p>
                <p className="text-blue-700">
                  <strong>Botón:</strong> {activeButton || 'Ninguno'}
                </p>
              </div>
              
              <div className="bg-green-100 p-4 rounded-lg">
                <h3 className="font-semibold text-green-800 mb-2">🌟 Truco Detectado</h3>
                <p className="text-green-700">
                  <strong>Último truco:</strong> {detectedTrick || 'Ninguno'}
                </p>
                <p className="text-green-700">
                  <strong>Área de trucos:</strong> {isTrickAreaActive ? 'Activa' : 'Inactiva'}
                </p>
              </div>
            </div>
          </div>
          
          {/* Panel de Controles */}
          <div className="space-y-6">
            {/* Controles Básicos */}
            <div className="bg-gradient-to-br from-blue-600 to-blue-700 p-6 rounded-lg text-white">
              <h3 className="text-xl font-semibold mb-4">🎮 Controles Básicos</h3>
              <div className="space-y-3">
                <div className="bg-white/20 p-3 rounded-lg">
                  <div className="font-semibold text-blue-100">🕹️ Joystick Virtual</div>
                  <div className="text-sm text-blue-50">Mover jugador y apuntar</div>
                </div>
                <div className="bg-white/20 p-3 rounded-lg">
                  <div className="font-semibold text-blue-100">🔘 Botón PASE</div>
                  <div className="text-sm text-blue-50">Pasar balón a compañero</div>
                </div>
                <div className="bg-white/20 p-3 rounded-lg">
                  <div className="font-semibold text-blue-100">🔘 Botón DISPARO</div>
                  <div className="text-sm text-blue-50">Disparar a portería</div>
                </div>
                <div className="bg-white/20 p-3 rounded-lg">
                  <div className="font-semibold text-blue-100">🔘 Botón SPRINT</div>
                  <div className="text-sm text-blue-50">Correr más rápido</div>
                </div>
                <div className="bg-white/20 p-3 rounded-lg">
                  <div className="font-semibold text-blue-100">🔘 Botón TACKLE</div>
                  <div className="text-sm text-blue-50">Entrada/Robar balón</div>
                </div>
              </div>
            </div>
            
            {/* Área de Trucos */}
            <div className="bg-gradient-to-br from-purple-600 to-purple-700 p-6 rounded-lg text-white">
              <h3 className="text-xl font-semibold mb-4">🌟 Área de Trucos</h3>
              <div className="space-y-3">
                {Object.entries(trickPatterns).map(([trick, description]) => (
                  <div key={trick} className="bg-white/20 p-3 rounded-lg">
                    <div className="font-semibold text-purple-100">{trick}</div>
                    <div className="text-sm text-purple-50">{description}</div>
                  </div>
                ))}
              </div>
            </div>
            
            {/* Cómo Usar */}
            <div className="bg-gradient-to-br from-orange-600 to-orange-700 p-6 rounded-lg text-white">
              <h3 className="text-xl font-semibold mb-4">❓ Cómo Usar</h3>
              <div className="space-y-3 text-sm">
                <div className="bg-white/20 p-3 rounded-lg">
                  <strong>1.</strong> Usa el joystick para mover al jugador
                </div>
                <div className="bg-white/20 p-3 rounded-lg">
                  <strong>2.</strong> Presiona botones para acciones básicas
                </div>
                <div className="bg-white/20 p-3 rounded-lg">
                  <strong>3.</strong> Dibuja gestos SOLO en el área de trucos
                </div>
                <div className="bg-white/20 p-3 rounded-lg">
                  <strong>4.</strong> ¡Mucho más fácil e intuitivo!
                </div>
              </div>
            </div>
          </div>
        </div>
        
        {/* Ventajas del Nuevo Sistema */}
        <div className="mt-8 bg-gradient-to-r from-green-500 to-blue-500 p-6 rounded-lg text-white">
          <h2 className="text-2xl font-bold mb-4 text-center">
            🚀 ¡Nuevo Sistema de Controles Híbridos!
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="text-center">
              <div className="text-3xl mb-2">🎯</div>
              <div className="font-semibold">Más Preciso</div>
              <div className="text-sm">Joystick para apuntar exactamente</div>
            </div>
            <div className="text-center">
              <div className="text-3xl mb-2">⚡</div>
              <div className="font-semibold">Más Rápido</div>
              <div className="text-sm">Botones instantáneos para acciones</div>
            </div>
            <div className="text-center">
              <div className="text-3xl mb-2">🎮</div>
              <div className="font-semibold">Más Intuitivo</div>
              <div className="text-sm">Trucos solo donde corresponde</div>
            </div>
          </div>
        </div>
        
        {/* Nota Importante */}
        <div className="mt-8 bg-green-50 border-l-4 border-green-400 p-6 rounded-lg">
          <div className="flex items-center">
            <div className="text-green-400 text-2xl mr-4">✅</div>
            <div>
              <h3 className="text-lg font-semibold text-green-800">¡Perfecto! Sistema Mejorado</h3>
              <p className="text-green-700 mt-2">
                Este nuevo sistema híbrido combina lo mejor de ambos mundos:
                <strong> joystick virtual y botones</strong> para acciones básicas,
                <strong> gestos táctiles</strong> solo para trucos en su área específica.
                ¡Mucho más fácil de usar y controlar!
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TouchControlsDemo;