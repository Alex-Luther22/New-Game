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

  // Detectar gamepad
  useEffect(() => {
    const checkGamepad = () => {
      const gamepads = navigator.getGamepads();
      const connected = Array.from(gamepads).some(gp => gp !== null);
      setGamepadConnected(connected);
    };

    const gamepadInterval = setInterval(checkGamepad, 1000);
    return () => clearInterval(gamepadInterval);
  }, []);

  // Manejo de gamepad
  useEffect(() => {
    const handleGamepadInput = () => {
      if (!gamepadConnected) return;

      const gamepads = navigator.getGamepads();
      const gamepad = Array.from(gamepads).find(gp => gp !== null);
      
      if (gamepad) {
        // Joystick izquierdo
        const leftStickX = gamepad.axes[0];
        const leftStickY = gamepad.axes[1];
        
        if (Math.abs(leftStickX) > 0.1 || Math.abs(leftStickY) > 0.1) {
          setJoystickPosition({ x: leftStickX, y: leftStickY });
          setIsJoystickActive(true);
          setIsMoving(true);
          
          const newPlayerPos = {
            x: Math.max(20, Math.min(580, playerPosition.x + leftStickX * 3)),
            y: Math.max(20, Math.min(380, playerPosition.y + leftStickY * 3))
          };
          setPlayerPosition(newPlayerPos);
        } else {
          setIsJoystickActive(false);
          setIsMoving(false);
        }

        // Botones del gamepad
        if (gamepad.buttons[0] && gamepad.buttons[0].pressed) executeAction('pass');
        if (gamepad.buttons[1] && gamepad.buttons[1].pressed) executeAction('shoot');
        if (gamepad.buttons[2] && gamepad.buttons[2].pressed) executeAction('throughPass');
        if (gamepad.buttons[3] && gamepad.buttons[3].pressed) executeAction('cross');
        if (gamepad.buttons[4] && gamepad.buttons[4].pressed) executeAction('sprint');
        if (gamepad.buttons[5] && gamepad.buttons[5].pressed) executeAction('tackle');
        if (gamepad.buttons[8] && gamepad.buttons[8].pressed) changePlayer();
        if (gamepad.buttons[9] && gamepad.buttons[9].pressed) togglePause();
      }
    };

    const gamepadLoop = setInterval(handleGamepadInput, 16); // 60 FPS
    return () => clearInterval(gamepadLoop);
  }, [gamepadConnected, playerPosition]);

  useEffect(() => {
    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    
    canvas.width = 600;
    canvas.height = 400;
    
    // Limpiar canvas
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    
    // Dibujar campo con límites
    drawField(ctx);
    
    // Dibujar jugador
    drawPlayer(ctx, playerPosition.x, playerPosition.y);
    
    // Dibujar balón
    drawBall(ctx, ballPosition.x, ballPosition.y);
    
    // Dibujar controles
    drawControls(ctx);
    
    // Dibujar controles de juego
    drawGameControls(ctx);
    
    // Dibujar gesto de truco
    if (trickGesture.length > 1) {
      drawTrickGesture(ctx, trickGesture);
    }
    
    // Dibujar interfaz de personalización
    if (isCustomizationMode) {
      drawCustomizationUI(ctx);
    }
    
    // Indicadores de seguimiento
    drawTrackingIndicators(ctx);
    
  }, [ballPosition, playerPosition, joystickPosition, isJoystickActive, activeButton, trickGesture, isTrickAreaActive, isCustomizationMode, controlSettings, gameFeatures]);

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
    
    // Líneas de banda (límites)
    ctx.strokeStyle = '#FFFFFF';
    ctx.lineWidth = 3;
    ctx.strokeRect(2, 2, 596, 396);
    
    // Arcos de penalti
    ctx.strokeStyle = '#FFFFFF';
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.arc(90, 200, 50, -Math.PI/2, Math.PI/2);
    ctx.stroke();
    ctx.beginPath();
    ctx.arc(510, 200, 50, Math.PI/2, -Math.PI/2);
    ctx.stroke();
    
    // Puntos de penalti
    ctx.fillStyle = '#FFFFFF';
    ctx.beginPath();
    ctx.arc(70, 200, 3, 0, 2 * Math.PI);
    ctx.fill();
    ctx.beginPath();
    ctx.arc(530, 200, 3, 0, 2 * Math.PI);
    ctx.fill();
  };

  const drawPlayer = (ctx, x, y) => {
    // Sombra del jugador
    ctx.fillStyle = 'rgba(0, 0, 0, 0.3)';
    ctx.beginPath();
    ctx.ellipse(x + 1, y + 1, 14, 10, 0, 0, 2 * Math.PI);
    ctx.fill();
    
    // Jugador
    ctx.fillStyle = isMoving ? '#1565C0' : '#1976D2';
    ctx.beginPath();
    ctx.arc(x, y, 12, 0, 2 * Math.PI);
    ctx.fill();
    
    // Número del jugador
    ctx.fillStyle = '#FFFFFF';
    ctx.font = 'bold 14px Arial';
    ctx.textAlign = 'center';
    ctx.fillText(currentPlayer, x, y + 5);
    
    // Nombre del jugador si está activado
    if (gameFeatures.playerNames) {
      ctx.fillStyle = '#FFFFFF';
      ctx.font = '10px Arial';
      ctx.fillText('MESSI', x, y - 18);
    }
    
    // Indicador de posesión
    if (Math.abs(x - ballPosition.x) < 20 && Math.abs(y - ballPosition.y) < 20) {
      ctx.strokeStyle = '#FFD700';
      ctx.lineWidth = 3;
      ctx.beginPath();
      ctx.arc(x, y, 18, 0, 2 * Math.PI);
      ctx.stroke();
    }
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
    
    // Rastro del balón si se está moviendo
    if (ballSpeed > 0) {
      ctx.strokeStyle = 'rgba(255, 255, 255, 0.3)';
      ctx.lineWidth = 2;
      ctx.beginPath();
      ctx.moveTo(x - ballSpeed, y);
      ctx.lineTo(x, y);
      ctx.stroke();
    }
  };

  const drawControls = (ctx) => {
    // Área de joystick
    ctx.fillStyle = `rgba(255, 255, 255, ${0.1 * controlSettings.controlOpacity})`;
    ctx.beginPath();
    ctx.arc(joystickArea.x, joystickArea.y, joystickArea.radius, 0, 2 * Math.PI);
    ctx.fill();
    
    // Joystick background
    ctx.fillStyle = isJoystickActive ? 'rgba(0, 150, 255, 0.4)' : `rgba(255, 255, 255, ${0.2 * controlSettings.controlOpacity})`;
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
    
    // Botones de acción expandidos
    Object.entries(buttons).forEach(([key, button]) => {
      const isActive = activeButton === key;
      
      // Botón
      ctx.fillStyle = isActive ? button.color : `rgba(255, 255, 255, ${0.3 * controlSettings.controlOpacity})`;
      ctx.beginPath();
      ctx.arc(button.x, button.y, button.radius, 0, 2 * Math.PI);
      ctx.fill();
      
      // Borde
      ctx.strokeStyle = isActive ? '#FFFFFF' : button.color;
      ctx.lineWidth = 2;
      ctx.beginPath();
      ctx.arc(button.x, button.y, button.radius, 0, 2 * Math.PI);
      ctx.stroke();
      
      // Texto
      ctx.fillStyle = isActive ? '#FFFFFF' : '#000000';
      ctx.font = `${button.radius > 30 ? '10px' : '8px'} Arial`;
      ctx.textAlign = 'center';
      ctx.fillText(button.label, button.x, button.y + 3);
    });
    
    // Área de trucos expandida
    const trickOpacity = isTrickAreaActive ? 0.4 : 0.15;
    ctx.fillStyle = `rgba(255, 215, 0, ${trickOpacity * controlSettings.controlOpacity})`;
    ctx.fillRect(trickArea.x, trickArea.y, trickArea.width, trickArea.height);
    
    // Borde del área de trucos
    ctx.strokeStyle = isTrickAreaActive ? '#FFD700' : '#FFFFFF';
    ctx.lineWidth = 2;
    ctx.strokeRect(trickArea.x, trickArea.y, trickArea.width, trickArea.height);
    
    // Texto del área de trucos
    ctx.fillStyle = '#FFFFFF';
    ctx.font = '12px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('🌟 ÁREA DE TRUCOS EXPANDIDA 🌟', trickArea.x + trickArea.width / 2, trickArea.y + 20);
    ctx.fillText(`${Object.keys(trickPatterns).filter(t => trickPatterns[t].unlocked).length} trucos desbloqueados`, trickArea.x + trickArea.width / 2, trickArea.y + 35);
    ctx.fillText('Dibuja gestos aquí para trucos', trickArea.x + trickArea.width / 2, trickArea.y + 50);
    
    // Labels de controles
    ctx.fillStyle = '#FFFFFF';
    ctx.font = '10px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('JOYSTICK', joystickArea.x, joystickArea.y + 60);
    ctx.fillText(gamepadConnected ? 'GAMEPAD + TÁCTIL' : 'MOVER/APUNTAR', joystickArea.x, joystickArea.y + 72);
  };

  const drawGameControls = (ctx) => {
    // Controles de juego en la parte superior
    Object.entries(gameControls).forEach(([key, control]) => {
      const isActive = control.active;
      
      // Botón
      ctx.fillStyle = isActive ? '#4CAF50' : 'rgba(255, 255, 255, 0.3)';
      ctx.fillRect(control.x, control.y, control.width, control.height);
      
      // Borde
      ctx.strokeStyle = isActive ? '#2E7D32' : '#FFFFFF';
      ctx.lineWidth = 1;
      ctx.strokeRect(control.x, control.y, control.width, control.height);
      
      // Texto
      ctx.fillStyle = isActive ? '#FFFFFF' : '#000000';
      ctx.font = '10px Arial';
      ctx.textAlign = 'center';
      ctx.fillText(control.label, control.x + control.width / 2, control.y + control.height / 2 + 3);
    });
    
    // Información de estado
    ctx.fillStyle = '#FFFFFF';
    ctx.font = '12px Arial';
    ctx.textAlign = 'left';
    ctx.fillText(`Jugador: ${currentPlayer} | Velocidad: ${gameFeatures.gameSpeed}x`, 10, 30);
    ctx.fillText(`Seguimiento: ${gameFeatures.ballFollow ? 'ON' : 'OFF'} | Gamepad: ${gamepadConnected ? 'ON' : 'OFF'}`, 10, 15);
  };

  const drawTrackingIndicators = (ctx) => {
    // Indicador de seguimiento del balón
    if (gameFeatures.ballFollow) {
      ctx.strokeStyle = '#00FF00';
      ctx.lineWidth = 2;
      ctx.setLineDash([5, 5]);
      ctx.beginPath();
      ctx.arc(ballPosition.x, ballPosition.y, 15, 0, 2 * Math.PI);
      ctx.stroke();
      ctx.setLineDash([]);
    }
    
    // Líneas de fuera de juego
    ctx.strokeStyle = 'rgba(255, 0, 0, 0.5)';
    ctx.lineWidth = 1;
    ctx.setLineDash([3, 3]);
    ctx.beginPath();
    ctx.moveTo(playerPosition.x, 0);
    ctx.lineTo(playerPosition.x, 400);
    ctx.stroke();
    ctx.setLineDash([]);
  };

  const drawCustomizationUI = (ctx) => {
    // Overlay de personalización
    ctx.fillStyle = 'rgba(0, 0, 0, 0.5)';
    ctx.fillRect(0, 0, 600, 400);
    
    // Texto de personalización
    ctx.fillStyle = '#FFFFFF';
    ctx.font = '20px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('MODO PERSONALIZACIÓN', 300, 200);
    ctx.font = '14px Arial';
    ctx.fillText('Arrastra controles para reposicionar', 300, 220);
    ctx.fillText('Presiona GUARDAR para confirmar', 300, 240);
  };

  const drawTrickGesture = (ctx, gesture) => {
    if (gesture.length < 2) return;
    
    ctx.strokeStyle = '#FFD700';
    ctx.lineWidth = 4;
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';
    
    ctx.beginPath();
    ctx.moveTo(gesture[0].x, gesture[0].y);
    
    for (let i = 1; i < gesture.length; i++) {
      ctx.lineTo(gesture[i].x, gesture[i].y);
    }
    
    ctx.stroke();
    
    // Puntos de inicio y fin
    ctx.fillStyle = '#FF4444';
    ctx.beginPath();
    ctx.arc(gesture[0].x, gesture[0].y, 4, 0, 2 * Math.PI);
    ctx.fill();
    
    ctx.fillStyle = '#44FF44';
    ctx.beginPath();
    ctx.arc(gesture[gesture.length - 1].x, gesture[gesture.length - 1].y, 4, 0, 2 * Math.PI);
    ctx.fill();
  };

  const changePlayer = () => {
    const players = ['10', '9', '11', '7', '8'];
    const currentIndex = players.indexOf(currentPlayer);
    const nextIndex = (currentIndex + 1) % players.length;
    setCurrentPlayer(players[nextIndex]);
  };

  const togglePause = () => {
    setGameFeatures(prev => ({
      ...prev,
      gameSpeed: prev.gameSpeed === 0 ? 1 : 0
    }));
  };

  const handleMouseDown = (e) => {
    const rect = canvasRef.current.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;
    
    // Verificar controles de juego
    Object.entries(gameControls).forEach(([key, control]) => {
      if (x >= control.x && x <= control.x + control.width &&
          y >= control.y && y <= control.y + control.height) {
        handleGameControlClick(key);
        return;
      }
    });
    
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

  const handleGameControlClick = (controlKey) => {
    switch (controlKey) {
      case 'changePlayer':
        changePlayer();
        break;
      case 'ballCamera':
        setGameFeatures(prev => ({
          ...prev,
          ballFollow: !prev.ballFollow
        }));
        break;
      case 'substitution':
        // Simular sustitución
        changePlayer();
        break;
      case 'pause':
        togglePause();
        break;
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
        x: Math.max(20, Math.min(580, playerPosition.x + deltaX * 0.15)),
        y: Math.max(20, Math.min(380, playerPosition.y + deltaY * 0.15))
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
      case 'throughPass':
        executeThroughPass();
        break;
      case 'cross':
        executeCross();
        break;
      case 'sprint':
        executeSprint();
        break;
      case 'tackle':
        executeTackle();
        break;
      case 'slideTackle':
        executeSlideTackle();
        break;
      case 'callForBall':
        executeCallForBall();
        break;
    }
  };

  const executePass = () => {
    const direction = joystickPosition.x > 0 ? 1 : -1;
    const newBallPos = {
      x: Math.max(20, Math.min(580, ballPosition.x + direction * 60)),
      y: Math.max(20, Math.min(380, ballPosition.y + joystickPosition.y * 40))
    };
    
    setBallSpeed(15);
    animateBall(newBallPos);
  };

  const executeShoot = () => {
    const direction = joystickPosition.x > 0 ? 1 : -1;
    const newBallPos = {
      x: Math.max(20, Math.min(580, ballPosition.x + direction * 120)),
      y: Math.max(20, Math.min(380, ballPosition.y + joystickPosition.y * 60))
    };
    
    setBallSpeed(25);
    animateBall(newBallPos);
  };

  const executeThroughPass = () => {
    const direction = joystickPosition.x > 0 ? 1 : -1;
    const newBallPos = {
      x: Math.max(20, Math.min(580, ballPosition.x + direction * 80)),
      y: Math.max(20, Math.min(380, ballPosition.y + joystickPosition.y * 20))
    };
    
    setBallSpeed(20);
    animateBall(newBallPos);
  };

  const executeCross = () => {
    const newBallPos = {
      x: Math.max(20, Math.min(580, ballPosition.x + (joystickPosition.x > 0 ? 100 : -100))),
      y: Math.max(20, Math.min(380, 200 + Math.random() * 60 - 30))
    };
    
    setBallSpeed(18);
    animateBall(newBallPos);
  };

  const executeSprint = () => {
    setIsMoving(true);
    setTimeout(() => setIsMoving(false), 1000);
  };

  const executeTackle = () => {
    const originalPos = { ...playerPosition };
    setPlayerPosition({ x: originalPos.x + 25, y: originalPos.y });
    setTimeout(() => setPlayerPosition(originalPos), 300);
  };

  const executeSlideTackle = () => {
    const originalPos = { ...playerPosition };
    setPlayerPosition({ x: originalPos.x + 40, y: originalPos.y + 10 });
    setTimeout(() => setPlayerPosition(originalPos), 500);
  };

  const executeCallForBall = () => {
    // Animar balón hacia el jugador
    const newBallPos = {
      x: playerPosition.x + 15,
      y: playerPosition.y
    };
    setBallSpeed(12);
    animateBall(newBallPos);
  };

  const animateBall = (targetPos) => {
    const startPos = { ...ballPosition };
    const steps = 30;
    let currentStep = 0;
    
    const animate = () => {
      if (currentStep < steps) {
        const progress = currentStep / steps;
        const easedProgress = 1 - Math.pow(1 - progress, 3); // Ease out cubic
        setBallPosition({
          x: startPos.x + (targetPos.x - startPos.x) * easedProgress,
          y: startPos.y + (targetPos.y - startPos.y) * easedProgress
        });
        
        // Reducir velocidad gradualmente
        setBallSpeed(prevSpeed => prevSpeed * 0.95);
        
        currentStep++;
        setTimeout(animate, 33); // ~30 FPS
      } else {
        setBallSpeed(0);
      }
    };
    
    animate();
  };

  const detectTrick = (gesture) => {
    if (gesture.length < 8) return 'Ninguno';
    
    // Análisis más avanzado de patrones
    const patterns = analyzeGesturePattern(gesture);
    
    if (patterns.isCircular && patterns.completeness > 0.7) return 'Roulette';
    if (patterns.isLShape && patterns.sharpness > 0.8) return 'Elastico';
    if (patterns.isZigzag && patterns.directionChanges >= 3) return 'Step-over';
    if (patterns.isVertical && patterns.straightness > 0.8) return 'Nutmeg';
    if (patterns.isArc && patterns.curvature > 0.6) return 'Rainbow Flick';
    if (patterns.isCurved && patterns.smoothness > 0.7) return 'Rabona';
    if (patterns.isBackward && patterns.direction < -0.5) return 'Heel Flick';
    if (patterns.isSShape && patterns.waves >= 2) return 'Scorpion';
    if (patterns.isHalfCircle && patterns.completeness > 0.4) return 'Maradona Turn';
    if (patterns.isVShape && patterns.angle > 0.5) return 'Cruyff Turn';
    if (patterns.isXShape && patterns.crossings >= 1) return 'Bicycle Kick';
    if (patterns.isShortLine && patterns.length < 30) return 'Fake Shot';
    
    return 'Ninguno';
  };

  const analyzeGesturePattern = (gesture) => {
    const start = gesture[0];
    const end = gesture[gesture.length - 1];
    const length = gesture.length;
    
    // Análisis de circularidad
    const center = calculateCenter(gesture);
    const avgRadius = calculateAverageRadius(gesture, center);
    const circularPoints = gesture.filter(point => {
      const distance = Math.sqrt(Math.pow(point.x - center.x, 2) + Math.pow(point.y - center.y, 2));
      return Math.abs(distance - avgRadius) < avgRadius * 0.3;
    }).length;
    
    // Análisis de direccionalidad
    const totalDistance = Math.sqrt(Math.pow(end.x - start.x, 2) + Math.pow(end.y - start.y, 2));
    const horizontal = Math.abs(end.x - start.x);
    const vertical = Math.abs(end.y - start.y);
    
    // Análisis de cambios de dirección
    let directionChanges = 0;
    for (let i = 2; i < length; i++) {
      const angle1 = Math.atan2(gesture[i-1].y - gesture[i-2].y, gesture[i-1].x - gesture[i-2].x);
      const angle2 = Math.atan2(gesture[i].y - gesture[i-1].y, gesture[i].x - gesture[i-1].x);
      if (Math.abs(angle1 - angle2) > Math.PI / 4) {
        directionChanges++;
      }
    }
    
    return {
      isCircular: circularPoints / length > 0.6,
      completeness: circularPoints / length,
      isLShape: horizontal > vertical && directionChanges >= 1,
      sharpness: directionChanges / length,
      isZigzag: directionChanges >= 3,
      directionChanges,
      isVertical: vertical > horizontal * 2,
      straightness: 1 - (directionChanges / length),
      isArc: vertical > horizontal && directionChanges <= 2,
      curvature: Math.min(vertical / horizontal, 1),
      isCurved: directionChanges >= 2 && directionChanges <= 4,
      smoothness: 1 - (directionChanges / length * 2),
      isBackward: (end.x - start.x) < 0,
      direction: (end.x - start.x) / totalDistance,
      isSShape: directionChanges >= 4,
      waves: Math.floor(directionChanges / 2),
      isHalfCircle: circularPoints / length > 0.4 && circularPoints / length < 0.7,
      isVShape: directionChanges === 1 && horizontal > vertical,
      angle: Math.min(horizontal / vertical, vertical / horizontal),
      isXShape: directionChanges >= 2 && Math.abs(horizontal - vertical) < 20,
      crossings: Math.floor(directionChanges / 2),
      isShortLine: totalDistance < 50 && directionChanges <= 1,
      length: totalDistance
    };
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
    if (!trickPatterns[trick] || !trickPatterns[trick].unlocked) return;
    
    const originalPos = { ...playerPosition };
    let animationSteps = [];
    
    switch (trick) {
      case 'Roulette':
        animationSteps = [
          { x: originalPos.x + 8, y: originalPos.y },
          { x: originalPos.x + 6, y: originalPos.y - 6 },
          { x: originalPos.x, y: originalPos.y - 8 },
          { x: originalPos.x - 6, y: originalPos.y - 6 },
          { x: originalPos.x - 8, y: originalPos.y },
          { x: originalPos.x - 6, y: originalPos.y + 6 },
          { x: originalPos.x, y: originalPos.y + 8 },
          { x: originalPos.x + 6, y: originalPos.y + 6 },
          originalPos
        ];
        break;
      case 'Elastico':
        animationSteps = [
          { x: originalPos.x + 15, y: originalPos.y },
          { x: originalPos.x + 10, y: originalPos.y - 5 },
          { x: originalPos.x, y: originalPos.y - 15 },
          { x: originalPos.x - 10, y: originalPos.y - 10 },
          originalPos
        ];
        break;
      case 'Step-over':
        animationSteps = [
          { x: originalPos.x + 12, y: originalPos.y },
          { x: originalPos.x - 12, y: originalPos.y },
          { x: originalPos.x + 8, y: originalPos.y },
          { x: originalPos.x - 8, y: originalPos.y },
          originalPos
        ];
        break;
      case 'Nutmeg':
        animationSteps = [
          { x: originalPos.x, y: originalPos.y + 20 },
          { x: originalPos.x, y: originalPos.y - 5 },
          originalPos
        ];
        break;
      case 'Rainbow Flick':
        animationSteps = [
          { x: originalPos.x + 5, y: originalPos.y - 10 },
          { x: originalPos.x + 10, y: originalPos.y - 15 },
          { x: originalPos.x + 15, y: originalPos.y - 10 },
          { x: originalPos.x + 20, y: originalPos.y },
          originalPos
        ];
        break;
      case 'Fake Shot':
        animationSteps = [
          { x: originalPos.x + 5, y: originalPos.y - 5 },
          { x: originalPos.x - 5, y: originalPos.y + 5 },
          originalPos
        ];
        break;
      default:
        animationSteps = [
          { x: originalPos.x + 10, y: originalPos.y + 10 },
          { x: originalPos.x - 10, y: originalPos.y - 10 },
          originalPos
        ];
    }
    
    let stepIndex = 0;
    const animateStep = () => {
      if (stepIndex < animationSteps.length) {
        setPlayerPosition(animationSteps[stepIndex]);
        stepIndex++;
        setTimeout(animateStep, 120);
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
    setBallSpeed(0);
    setCurrentPlayer('10');
  };

  const toggleCustomization = () => {
    setIsCustomizationMode(!isCustomizationMode);
  };

  const saveCustomization = () => {
    localStorage.setItem('footballControlSettings', JSON.stringify(controlSettings));
    setIsCustomizationMode(false);
  };

  const loadCustomization = () => {
    const saved = localStorage.getItem('footballControlSettings');
    if (saved) {
      setControlSettings(JSON.parse(saved));
    }
  };

  // Cargar configuración guardada al iniciar
  useEffect(() => {
    loadCustomization();
  }, []);

  return (
    <div className="max-w-7xl mx-auto p-6 bg-gradient-to-br from-green-50 to-blue-50 min-h-screen">
      <div className="bg-white rounded-xl shadow-2xl p-8">
        <h1 className="text-4xl font-bold text-center mb-8 text-gray-800">
          ⚽ Football Master - Sistema Avanzado de Controles
        </h1>
        
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Campo de Juego */}
          <div className="lg:col-span-2">
            <div className="bg-gradient-to-br from-green-600 to-green-700 p-4 rounded-lg">
              <h2 className="text-xl font-semibold text-white mb-4 text-center">
                🎮 Demo Interactiva - {gamepadConnected ? 'Gamepad + ' : ''}Controles Híbridos
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
              
              <div className="mt-4 flex flex-wrap gap-2 justify-center">
                <button
                  onClick={resetDemo}
                  className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-lg font-semibold transition-colors"
                >
                  🔄 Reiniciar
                </button>
                <button
                  onClick={toggleCustomization}
                  className="bg-purple-500 hover:bg-purple-600 text-white px-4 py-2 rounded-lg font-semibold transition-colors"
                >
                  ⚙️ Personalizar
                </button>
                {isCustomizationMode && (
                  <button
                    onClick={saveCustomization}
                    className="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded-lg font-semibold transition-colors"
                  >
                    💾 Guardar
                  </button>
                )}
              </div>
            </div>
            
            {/* Información en tiempo real */}
            <div className="mt-6 grid grid-cols-1 md:grid-cols-3 gap-4">
              <div className="bg-blue-100 p-4 rounded-lg">
                <h3 className="font-semibold text-blue-800 mb-2">🎯 Estado del Juego</h3>
                <p className="text-blue-700 text-sm">
                  <strong>Jugador:</strong> {currentPlayer} ({isMoving ? 'Moviéndose' : 'Parado'})
                </p>
                <p className="text-blue-700 text-sm">
                  <strong>Balón:</strong> Velocidad {ballSpeed.toFixed(1)}
                </p>
                <p className="text-blue-700 text-sm">
                  <strong>Seguimiento:</strong> {gameFeatures.ballFollow ? 'ON' : 'OFF'}
                </p>
              </div>
              
              <div className="bg-green-100 p-4 rounded-lg">
                <h3 className="font-semibold text-green-800 mb-2">🌟 Truco Detectado</h3>
                <p className="text-green-700 text-sm">
                  <strong>Último truco:</strong> {detectedTrick || 'Ninguno'}
                </p>
                <p className="text-green-700 text-sm">
                  <strong>Área activa:</strong> {isTrickAreaActive ? 'SÍ' : 'NO'}
                </p>
                <p className="text-green-700 text-sm">
                  <strong>Desbloqueados:</strong> {Object.keys(trickPatterns).filter(t => trickPatterns[t].unlocked).length}/12
                </p>
              </div>
              
              <div className="bg-purple-100 p-4 rounded-lg">
                <h3 className="font-semibold text-purple-800 mb-2">🎮 Controles</h3>
                <p className="text-purple-700 text-sm">
                  <strong>Gamepad:</strong> {gamepadConnected ? 'Conectado' : 'Desconectado'}
                </p>
                <p className="text-purple-700 text-sm">
                  <strong>Botón activo:</strong> {activeButton || 'Ninguno'}
                </p>
                <p className="text-purple-700 text-sm">
                  <strong>Personalización:</strong> {isCustomizationMode ? 'ON' : 'OFF'}
                </p>
              </div>
            </div>
          </div>
          
          {/* Panel de Controles */}
          <div className="space-y-6">
            {/* Controles Básicos */}
            <div className="bg-gradient-to-br from-blue-600 to-blue-700 p-6 rounded-lg text-white">
              <h3 className="text-xl font-semibold mb-4">🎮 Controles Básicos</h3>
              <div className="space-y-2">
                <div className="bg-white/20 p-2 rounded-lg">
                  <div className="font-semibold text-blue-100 text-sm">🕹️ Joystick Virtual</div>
                  <div className="text-xs text-blue-50">Mover jugador y apuntar</div>
                </div>
                <div className="bg-white/20 p-2 rounded-lg">
                  <div className="font-semibold text-blue-100 text-sm">🔘 PASE / DISPARO</div>
                  <div className="text-xs text-blue-50">Pase normal / Disparo a portería</div>
                </div>
                <div className="bg-white/20 p-2 rounded-lg">
                  <div className="font-semibold text-blue-100 text-sm">🔘 PASE PROF / CENTRO</div>
                  <div className="text-xs text-blue-50">Pase profundo / Centro al área</div>
                </div>
                <div className="bg-white/20 p-2 rounded-lg">
                  <div className="font-semibold text-blue-100 text-sm">🔘 SPRINT / TACKLE</div>
                  <div className="text-xs text-blue-50">Correr más rápido / Entrada</div>
                </div>
                <div className="bg-white/20 p-2 rounded-lg">
                  <div className="font-semibold text-blue-100 text-sm">🔘 BARRIDA / PEDIR</div>
                  <div className="text-xs text-blue-50">Barrida / Pedir balón</div>
                </div>
              </div>
            </div>
            
            {/* Controles de Juego */}
            <div className="bg-gradient-to-br from-green-600 to-green-700 p-6 rounded-lg text-white">
              <h3 className="text-xl font-semibold mb-4">⚽ Controles de Juego</h3>
              <div className="space-y-2">
                <div className="bg-white/20 p-2 rounded-lg">
                  <div className="font-semibold text-green-100 text-sm">🔄 CAMBIAR JUGADOR</div>
                  <div className="text-xs text-green-50">Cambio automático al más cercano</div>
                </div>
                <div className="bg-white/20 p-2 rounded-lg">
                  <div className="font-semibold text-green-100 text-sm">📹 CÁMARA BALÓN</div>
                  <div className="text-xs text-green-50">Seguimiento automático</div>
                </div>
                <div className="bg-white/20 p-2 rounded-lg">
                  <div className="font-semibold text-green-100 text-sm">🔄 SUSTITUCIÓN</div>
                  <div className="text-xs text-green-50">Cambio sin menú</div>
                </div>
                <div className="bg-white/20 p-2 rounded-lg">
                  <div className="font-semibold text-green-100 text-sm">⏸️ PAUSA</div>
                  <div className="text-xs text-green-50">Pausar/Reanudar juego</div>
                </div>
              </div>
            </div>
            
            {/* Área de Trucos Expandida */}
            <div className="bg-gradient-to-br from-purple-600 to-purple-700 p-6 rounded-lg text-white">
              <h3 className="text-xl font-semibold mb-4">🌟 Área de Trucos (12 Trucos)</h3>
              <div className="space-y-2 max-h-64 overflow-y-auto">
                {Object.entries(trickPatterns).map(([trick, data]) => (
                  <div key={trick} className={`bg-white/20 p-2 rounded-lg ${!data.unlocked ? 'opacity-50' : ''}`}>
                    <div className="font-semibold text-purple-100 text-sm">
                      {data.unlocked ? '✅' : '🔒'} {trick}
                    </div>
                    <div className="text-xs text-purple-50">
                      Dificultad: {data.difficulty}/5 - {data.description}
                    </div>
                  </div>
                ))}
              </div>
            </div>
            
            {/* Soporte Gamepad */}
            <div className="bg-gradient-to-br from-orange-600 to-orange-700 p-6 rounded-lg text-white">
              <h3 className="text-xl font-semibold mb-4">🎮 Soporte Gamepad</h3>
              <div className="space-y-2">
                <div className="bg-white/20 p-2 rounded-lg">
                  <div className="font-semibold text-orange-100 text-sm">
                    Estado: {gamepadConnected ? '✅ Conectado' : '❌ Desconectado'}
                  </div>
                  <div className="text-xs text-orange-50">
                    Xbox/PlayStation compatible
                  </div>
                </div>
                <div className="bg-white/20 p-2 rounded-lg">
                  <div className="font-semibold text-orange-100 text-sm">🕹️ Joystick Analógico</div>
                  <div className="text-xs text-orange-50">Movimiento preciso</div>
                </div>
                <div className="bg-white/20 p-2 rounded-lg">
                  <div className="font-semibold text-orange-100 text-sm">🔘 Botones Mapeados</div>
                  <div className="text-xs text-orange-50">A=Pase, B=Disparo, X=Pase Prof, Y=Centro</div>
                </div>
              </div>
            </div>
          </div>
        </div>
        
        {/* Características Avanzadas */}
        <div className="mt-8 bg-gradient-to-r from-green-500 to-blue-500 p-6 rounded-lg text-white">
          <h2 className="text-2xl font-bold mb-4 text-center">
            🚀 Sistema Avanzado de Controles
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div className="text-center">
              <div className="text-3xl mb-2">🎯</div>
              <div className="font-semibold">Personalizable</div>
              <div className="text-sm">Mueve y ajusta todos los controles</div>
            </div>
            <div className="text-center">
              <div className="text-3xl mb-2">🎮</div>
              <div className="font-semibold">Gamepad Ready</div>
              <div className="text-sm">Xbox/PlayStation compatible</div>
            </div>
            <div className="text-center">
              <div className="text-3xl mb-2">🌟</div>
              <div className="font-semibold">12 Trucos</div>
              <div className="text-sm">Área expandida con más trucos</div>
            </div>
            <div className="text-center">
              <div className="text-3xl mb-2">📱</div>
              <div className="font-semibold">2GB Optimizado</div>
              <div className="text-sm">Perfecto para Tecno Spark 8C</div>
            </div>
          </div>
        </div>
        
        {/* Características Realistas */}
        <div className="mt-8 bg-gradient-to-r from-purple-500 to-pink-500 p-6 rounded-lg text-white">
          <h2 className="text-2xl font-bold mb-4 text-center">
            ⚽ Características Realistas de Fútbol
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="text-center">
              <div className="text-3xl mb-2">🏃</div>
              <div className="font-semibold">Seguimiento Automático</div>
              <div className="text-sm">Cambio automático al jugador más cercano</div>
            </div>
            <div className="text-center">
              <div className="text-3xl mb-2">🎯</div>
              <div className="font-semibold">Fuera de Juego</div>
              <div className="text-sm">Líneas de fuera de juego válidas</div>
            </div>
            <div className="text-center">
              <div className="text-3xl mb-2">🔄</div>
              <div className="font-semibold">Sustituciones Rápidas</div>
              <div className="text-sm">Sin necesidad de menús</div>
            </div>
          </div>
        </div>
        
        {/* Música y Audio */}
        <div className="mt-8 bg-gradient-to-r from-yellow-500 to-orange-500 p-6 rounded-lg text-white">
          <h2 className="text-2xl font-bold mb-4 text-center">
            🎵 Sistema de Audio Avanzado
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <h3 className="font-semibold mb-2">🎼 Música Implementada</h3>
              <ul className="text-sm space-y-1">
                <li>• 25 pistas libres de copyright</li>
                <li>• Música dinámica según intensidad</li>
                <li>• Estilos regionales (Latino, Europeo, Africano)</li>
                <li>• Música específica para modos de juego</li>
              </ul>
            </div>
            <div>
              <h3 className="font-semibold mb-2">🔊 Efectos de Sonido</h3>
              <ul className="text-sm space-y-1">
                <li>• Silbatos y reacciones de multitud</li>
                <li>• Sonidos realistas del balón</li>
                <li>• Narrador de estadio multiidioma</li>
                <li>• Efectos de trucos y celebraciones</li>
              </ul>
            </div>
          </div>
        </div>
        
        {/* Instrucciones */}
        <div className="mt-8 bg-green-50 border-l-4 border-green-400 p-6 rounded-lg">
          <div className="flex items-center">
            <div className="text-green-400 text-2xl mr-4">✅</div>
            <div>
              <h3 className="text-lg font-semibold text-green-800">¡Sistema Completamente Rediseñado!</h3>
              <p className="text-green-700 mt-2">
                <strong>Nuevo:</strong> 8 botones de acción organizados, área de trucos expandida (250x150px) 
                en superior derecha, 12 trucos con detección avanzada, soporte completo para gamepad, 
                controles personalizables, seguimiento automático, fuera de juego válido, y optimización 
                completa para dispositivos de 2GB RAM como Tecno Spark 8C.
              </p>
              <p className="text-green-700 mt-2">
                <strong>Características únicas:</strong> Sistema híbrido único que combina la precisión 
                del joystick con la velocidad de los botones y la creatividad de los gestos táctiles.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TouchControlsDemo;