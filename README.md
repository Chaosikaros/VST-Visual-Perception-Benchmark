# VST Visual Perception Benchmark

A low-cost, accessible framework for measuring and comparing the visual perception performance of Video See-Through (VST) head-mounted displays (HMDs) against natural human vision.

## Overview

Video see-through (VST) technology aims to seamlessly blend virtual and physical worlds by reconstructing reality through cameras. However, it remains unclear how close these systems are to replicating natural human vision across varying environmental conditions. This benchmark provides a standardized method to quantify the perceptual gap between VST headsets and the human eye.

### What Is It For?

This benchmark is designed to:

- **Evaluate VST HMDs**: Assess the visual perception capabilities of VST headsets (e.g., Apple Vision Pro, Meta Quest 3, Quest Pro) across different lighting conditions
- **Compare with Human Vision**: Quantitatively measure how VST systems perform relative to natural naked eye vision
- **Identify Limitations**: Reveal specific areas where current VST technology falls short, particularly in low-light environments
- **Guide Development**: Provide actionable metrics that can inform the design and optimization of future VST HMDs
- **Support Research**: Offer a standardized, replicable methodology for visual perception assessment in mixed reality systems

### Principles

The benchmark adapts psychophysical methods from vision science to evaluate three fundamental aspects of visual perception:

#### 1. **Visual Acuity** 
Visual acuity measures the clarity or sharpness of vision—the ability to discern fine details. It is quantified using:
- **Visual Angle**: The angle an object subtends at the eye: `Visual Angle = 2 × arctan(Object Size / (2 × Object Distance))`
- **MAR (Minimum Angle of Resolution)**: The smallest gap size that can be resolved, measured in arc minutes
- **Decimal Visual Acuity**: The reciprocal of MAR (normal vision = 1.0)
- **logMAR**: Logarithm of MAR (normal vision = 0)
- **Snellen Fraction**: Traditional notation like 20/20 vision

The test uses a **Tumbling E Chart** with a bisection method to precisely determine the smallest recognizable optotype size.

#### 2. **Contrast Sensitivity**
Contrast sensitivity measures the ability to discern differences between light and dark, particularly important in low-light conditions and for detecting fine details. It is quantified using:
- **Weber Contrast**: `(Luminance_target - Luminance_background) / Luminance_background`
- **Contrast Sensitivity**: The inverse of the contrast threshold
- **logCS**: Logarithm of contrast sensitivity (normal = 2.0 for Pelli-Robson)

The test uses a **Pelli-Robson Chart** that presents letters at fixed size but decreasing contrast levels.

#### 3. **Color Vision**
Color vision assesses the ability to distinguish different hues and perceive accurate color representation. The test uses:
- **Farnsworth-Munsell 100 Hue Test**: Participants arrange 85 colored caps in correct hue order across 4 groups
- **Total Error Score (TES)**: Calculated by summing placement errors: `TES = Σ|O(i-1) - O(i)| + |O(i+1) - O(i)| - 2`

A lower error score indicates better color discrimination ability.

## Installation

### Requirements
- **Unity 6000.0.39f1 or later**
- **Platform Support**:
  - PC - All tests supported
  - Android - Visual acuity and contrast sensitivity tests only

### Setup
1. Clone this repository
2. Open the project in Unity
3. Load the desired test scene from `Assets/Scenes/`:
   - `Visual acuity test.unity`
   - `Contrast sensitivity test.unity`
   - `Color vision test.unity`

## How to Use

### General Workflow

All tests follow a similar workflow:

1. **Configure Participant Information**
   - Select Participant ID (1-64)
   - Choose Device type:
     - `eyes` - Testing naked eye vision
     - `A`, `B`, `C` - Testing different VST HMDs
   - Select Trial type:
     - `Both` - Standard test
     - Custom trial conditions

2. **Start the Test**
   - Click the "Start" button to begin
   - Follow the on-screen instructions for each specific test

3. **Complete the Test**
   - Results are automatically saved to CSV files in the `Recordings` folder

### Test 1: Visual Acuity Test

**Platform**: PC and Android

**Purpose**: Measures the smallest visual detail that can be resolved at a given distance.

**How to Use**:
1. Launch the `Visual acuity test` scene
2. **Calibration** (first-time setup):
   - Click "Calibration" button
   - Adjust the vertical and horizontal sliders to center the E optotype
   - Adjust the **Size slider** on the right until the letter "E" has a **physical height of 2.4 cm** on the screen
   - Click "Save" to store calibration settings
3. **Testing**:
   - Maintain a viewing distance of **1 meter** from the screen
   - Configure participant ID, device, and trial
   - Click "Start Test"
   - A single letter "E" will appear in one of four orientations (↑↓←→)
   - Press the arrow key corresponding to the E's orientation
   - The E size will adjust using a bisection algorithm based on your responses
   - Continue until the test converges on your visual acuity threshold

**Output**: See [Output Format](#output-format) section below.

### Test 2: Contrast Sensitivity Test

**Platform**: PC and Android

**Purpose**: Measures the ability to detect low-contrast patterns, crucial for vision in varying lighting conditions.

**How to Use**:
1. Launch the `Contrast sensitivity test` scene
2. **Calibration** (first-time setup):
   - Ensure the display brightness is set to 100%
3. **Testing**:
   - Maintain a viewing distance of **1 meter** from the screen
   - Configure participant ID, device, and trial
   - Click "Start Test"
4. Two letters will appear with reduced contrast against the background
5. Type the two letters you see using your keyboard
6. The contrast level will adjust based on your responses using a bisection method
7. Continue until the test determines your contrast sensitivity threshold
8. Perform **Grayscale vs Weber Contrast** calibration later to ensure accurate contrast conversion

**Output**: See [Output Format](#output-format) section below.

### Test 3: Color Vision Test

**Platform**: PC ONLY (designed for calibrated monitors)

**Purpose**: Evaluates color discrimination ability and the accuracy of color representation through VST systems.

**How to Use**:
1. Launch the `Color vision test` scene on a PC
2. Ensure your monitor is properly calibrated for accurate color reproduction. Maintain a viewing distance of **50 cm** from the monitor
3. Configure participant ID, device, and trial
4. Click "Start Test"
5. You will see 4 rows of colored caps with fixed caps at each end
6. **Drag and drop** the colored caps to arrange them in order of hue
7. Each row should transition smoothly from the starting color to the ending color
8. Click "Submit" after arranging all rows
9. The test calculates your Total Error Score based on placement accuracy

**Output**: See [Output Format](#output-format) section below.

## Output Format

All test results are saved as CSV files in the following locations:
- **PC**: `Assets/Recordings/`
- **Android**: `Application.persistentDataPath/Recordings/`

### Visual Acuity Output

**Filename**: `VA_ID_{ParticipantID}_Device_{DeviceName}_Trail_{TrailType}{Timestamp}.csv`

**Columns**:
- `Timestamp`: Test execution timestamp (yyyy_MM_dd_hh_mm_ss_ffffff)
- `ID`: Participant ID (1-64)
- `Device`: Device type (eyes, A, B, C)
- `trail`: Trial condition
- `time interval`: Time between responses (seconds)
- `scale`: Current E optotype scale
- `currentMinScale`: Minimum scale in current bisection range
- `currentMaxScale`: Maximum scale in current bisection range
- `Width`: Screen width (pixels)
- `Height`: Screen height (pixels)
- `Correct Button`: Correct orientation (up/down/left/right)
- `Pressed Button`: User's response
- `Is Correct`: Whether response was correct (True/False)
- `VA`: Calculated visual acuity (decimal format)

### Contrast Sensitivity Output

**Filename**: `VCS_ID_{ParticipantID}_Device_{DeviceName}_Trail_{TrailType}{Timestamp}.csv`

**Columns**:
- `Timestamp`: Test execution timestamp
- `ID`: Participant ID
- `Device`: Device type
- `trail`: Trial condition
- `time interval`: Time between responses (seconds)
- `Virtual contrast`: Current contrast level (0-1)
- `currentMinContrast`: Minimum contrast in bisection range
- `currentMaxContrast`: Maximum contrast in bisection range
- `Correct letter`: Correct letter pair
- `Pressed letter`: User's input
- `Is Correct`: Whether response was correct (True/False)
- `Virtual contrast sensitivity`: Calculated contrast sensitivity (1/contrast)

### Color Vision Output

**Filename**: `VC_ID_{ParticipantID}_Device_{DeviceName}_Trail_{TrailType}{Timestamp}.csv`

**Columns**:
- `id`: Participant ID
- `currentDevice`: Device type
- `currentTrail`: Trial condition
- `row1`: Semicolon-separated cap indices for row 1
- `row2`: Semicolon-separated cap indices for row 2
- `row3`: Semicolon-separated cap indices for row 3
- `row4`: Semicolon-separated cap indices for row 4
- `errorScore`: Total Error Score (TES) - lower is better
- `test_time`: Total test duration (seconds)

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests to improve the benchmark.

## Contact

For questions or support, please open an issue on this repository.

Contact: chaosikaros@outlook.com