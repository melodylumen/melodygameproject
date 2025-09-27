# FMOD Studio Integration Setup

## Installation Instructions

1. **Download FMOD for Unity** (version 2.02+)
   - Visit: https://www.fmod.com/download
   - Download "FMOD for Unity"
   - Select version 2.02 or higher

2. **Import FMOD Package**
   ```
   Unity Menu → Assets → Import Package → Custom Package
   Select: FMODUnity.unitypackage
   ```

3. **Configure FMOD Settings**
   - Navigate to: FMOD → Edit Settings
   - Set Studio Project Path: `Assets/Audio/FMOD/RhythmSolitaire.fspro`
   - Set Build Path: `Assets/StreamingAssets/`
   - Enable: "Initialize FMOD Debug"
   - Set Platform Settings for PC/Switch/Mobile

4. **Constitutional Compliance Configuration**
   - Audio Latency Target: <20ms
   - Buffer Size: 128 samples (2.9ms at 44.1kHz)
   - Sample Rate: 44100 Hz
   - Speaker Mode: Stereo
   - DSP Buffer Count: 4 (for stability)

## Required FMOD Project Structure
```
Assets/Audio/FMOD/
├── RhythmSolitaire.fspro      # FMOD Studio project file
├── Build/                     # Compiled FMOD banks
│   ├── Desktop/
│   ├── Mobile/
│   └── Switch/
├── Metadata/                  # FMOD metadata
└── Events/                    # FMOD event references
```

## Constitutional Requirements
- **Audio Quality First**: Use FMOD Studio's high-quality resampling
- **Real-Time Performance**: Configure for <20ms latency
- **Memory Management**: Pre-allocate audio memory pools
- **Cross-Platform**: Platform-specific audio optimization

## Integration Verification
After setup, verify:
1. FMOD Studio can build banks without errors
2. Unity can load FMOD banks in play mode
3. Audio latency measurement shows <20ms
4. Memory allocation during audio playback is zero

## Next Steps
This setup will be completed when Unity Editor is available.
The FMOD project files and integration will be created according to this specification.