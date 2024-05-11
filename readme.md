In this project, I've developed an interactive sequencer with support for multiple instruments and timings, and a polyrhythm, both driven by a universal timer class. The setup includes several scripts and components that work together to synchronize audio playback with visual feedback and user interactions, and is intended to last 4 minutes.

**Initial Goals**: I wanted to create project that would have a strong connection between the visual elements and sound, such that the relationship would be easily recognizable to anyone. I opted to use a polyrhythm, however, to retain attention for longer periods of time (and to make it more interesting), I created a polyrhythm with 35 notes, where each note has a slightly longer interval than the last. The difference in timing is very small but grows over time. As the song progresses, some notes will naturally align, creating anticipation. This is supported by two main elements:

1. A collection of spheres that have rotation periods equal to the note intervals. These notes pulse a slightly brighter color when the kick drum gets triggered.
2. A circular array of boxes, where each box is offset on the y-axis by its corresponding note interval and a multiplier. The camera speeds through these boxes at the rate at which the root note is played, creating a morphing tunnel effect.

**Sequencer and Timing Control**: The `MainSequencerV2` script in Unity drives the sequencing logic, utilizing arrays to manage the timing and activation of audio samples handled in PureData. This script coordinates with `SongTimerV2`, which tracks the progression of the musical piece and ensures that all sequencing events are timed accurately.

**Dynamic Audio Effects**: Audio effects like reverb and delay are manipulated through Unity’s interface, with changes sent to PureData via the `LibPdInstance` script. This setup allows for real-time adjustments to the sound, which was helpful in tweaking the timings. These effects are triggered on a case-by-case basis.

**Visual Feedback Mechanism**: The `MainSequencerV2` script also manages some visual feedback, linking specific audio events (e.g., a kick drum hit) to visual changes such as altering the emission color of a GameObject’s material.

**Complex Time Synchronization**: The `SongTimerV2` script precisely calculates timings for audio and visual events, and each object references its timings for playback, instead of counters. This ensures that potential floating-point errors or system latency-type issues don't accumulate over time. This core class is shared with all other scripts.

Additionally, the polyrhythm script creates _an array of times for each note_ (list of _times_ within a list of _notes_) that indicates when it will be played. This is used not just for triggering samples in PureData, but also for transforming both the sphere and box visualizations.

**Structural and Navigational Flexibility**: The architecture of the system supports multiple instruments playing for different periods at different tempos during different sections, giving great flexibility. Each section can be configured with unique instrument patterns, effect settings, and corresponding visual behaviors through the `MainSequencerV2` script, providing flexibility in the composition and performance of the piece.

**Implementation Details**: The system uses specific patterns and settings for different instruments—like kicks, hi-hats, and bass—each managed in separate arrays within the `MainSequencerV2`. Instrument settings include volume, pitch patterns (for instruments like bass), and loop durations, all of which influence the sequencing and sound generation in PureData.
