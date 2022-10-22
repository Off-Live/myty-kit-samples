import { useEffect, useRef, useState } from 'react';
import './App.css';
import { FACEMESH_TESSELATION, Holistic } from '@mediapipe/holistic';
import { from } from 'rxjs';
import { drawConnectors } from '@mediapipe/drawing_utils';
import { Unity, useUnityContext } from 'react-unity-webgl';
import Select from 'react-select';
function App() {
  const holisticRef = useRef<Holistic|null>(null);
  const cameraRef = useRef<HTMLVideoElement>(null);
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [devices, setDevices] = useState<MediaDeviceInfo[]>([]);
  const [counter, setCounter] = useState(0);
  const [currentCam, setCurrentCam] = useState({ value: "", label: ""});
  

  const { unityProvider, isLoaded, sendMessage } = useUnityContext({
    loaderUrl: "webbuild/Build/webbuild.loader.js",
    dataUrl: "webbuild/Build/webbuild.data",
    frameworkUrl: "webbuild/Build/webbuild.framework.js",
    codeUrl: "webbuild/Build/webbuild.wasm",
  });


  useEffect(()=>{

    holisticRef.current = new Holistic({
      locateFile:(file) => {
        return `https://cdn.jsdelivr.net/npm/@mediapipe/holistic/${file}`;
      },
    })
    holisticRef.current.setOptions({
      selfieMode: true,
      minDetectionConfidence: 0.5,
      minTrackingConfidence: 0.5,
      modelComplexity: 1,
      smoothLandmarks:true
      
    })
    

    holisticRef.current.onResults((result)=>{
      const canvasCtx = canvasRef.current?.getContext('2d')!;
      canvasCtx.save();
      canvasCtx.clearRect(0, 0, canvasRef.current!.width, canvasRef.current!.height);
      drawConnectors(canvasCtx,result.faceLandmarks,FACEMESH_TESSELATION,{color: '#C0C0C070', lineWidth: 1});
      const motionData = {
        face: result.faceLandmarks,
        pose: result.poseLandmarks,
        width : cameraRef.current?.videoWidth,
        height : cameraRef.current?.videoHeight
      }
      sendMessage("WebInterface","ProcessCapturedData", JSON.stringify(motionData))
      setCounter((x) => x+1);
    })
  },[sendMessage,setCounter,currentCam])


  useEffect(() => {
    const device$ = from(navigator.mediaDevices.enumerateDevices());
    const subs = device$.subscribe((args)=>{
      setCurrentCam( { value : args[0].deviceId, label : args[0].label});
      setDevices(args);
    })
    
    return ()=>{
      subs.unsubscribe();
    }
  }, [setDevices, setCurrentCam]);

  useEffect(()=>{
    console.log(devices);
    if(devices.length===0) return;
    if(currentCam.value=='') return;

    const camera = devices.filter(dev=> dev.deviceId == currentCam.value && dev.kind=='videoinput')
    const lastSource = cameraRef.current;
    const subs = from(
        navigator.mediaDevices.getUserMedia({
          audio: false,
          video: { deviceId:camera[0].deviceId, width: 720, height: 480 },
        })
      ).subscribe((stream) => {
        cameraRef.current!.srcObject = stream;
        cameraRef.current!.play();
      });
    

    return ()=>{
      subs.unsubscribe();
      const stream = lastSource!.srcObject as MediaStream
      const tracks = stream!.getTracks();
      tracks.forEach((track) => {
        track.stop();
      });
    }
    
  }, [devices, currentCam])

  useEffect(()=>{
    if(counter>0)
      updateFunc();
  }, [counter])

  async function updateFunc() {
    if(cameraRef.current && holisticRef.current ){
      await holisticRef.current.send({image:cameraRef.current})
    }
  }

  function onFirstFrame(){

    updateFunc();
  }

  function onCameraChanged(item: { value: string; label: string }){
    setCurrentCam(item);
  }
  
  const option = devices.map((device: MediaDeviceInfo) => ({ value: device.deviceId, label: device.label }));

  return (
    <div className="App">
      <Select
        isSearchable = {false}
        onChange={onCameraChanged}
        options={option}
        value = {currentCam as any}
      />
      <video ref={cameraRef} autoPlay={true}  onLoadedData={onFirstFrame}  hidden={true}/>
      <canvas ref={canvasRef} width="720px" height="480px"/>
      <Unity style={{ visibility: isLoaded ? "visible" : "hidden", width:"720px", height:"480px"}}  unityProvider={unityProvider}/>
       
    </div>
  );
}

export default App;
