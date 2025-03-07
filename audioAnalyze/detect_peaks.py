from pydub import AudioSegment
from pydub.utils import make_chunks
import numpy as np
import os
from typing import List
import matplotlib.pyplot as plt

class AudioAnalyzer:
    def __init__(self, file_path: str, chunk_size_ms: int = 100, threshold_dbfs: float = -20):
        self.file_path = file_path
        self.chunk_size_ms = chunk_size_ms
        self.threshold_dbfs = threshold_dbfs
        self.audio = None
        self.chunks = None
        self.timestamps = []
        self.peak_indices = []  # To store the indices of peak chunks in each region

    def load_audio(self) -> bool:
        try:
            if not os.path.exists(self.file_path):
                print(f"Error: File not found at {self.file_path}")
                return False
            self.audio = AudioSegment.from_file(self.file_path)
            return True
        except Exception as e:
            print(f"Error loading audio file: {e}")
            return False

    def analyze(self) -> List[float]:
        if not self.audio:
            return []
        
        # Split audio into chunks
        self.chunks = make_chunks(self.audio, self.chunk_size_ms)
        timestamps = []

        # Variables to track a contiguous region above the threshold
        region_start = None
        region_max_dbfs = -float('inf')
        region_max_index = None

        # Iterate over chunks to find peaks
        for i, chunk in enumerate(self.chunks):
            if chunk.dBFS > self.threshold_dbfs:
                # Start a new region if not already in one
                if region_start is None:
                    region_start = i
                    region_max_dbfs = chunk.dBFS
                    region_max_index = i
                else:
                    # Update if the current chunk is louder than the previous maximum in the region
                    if chunk.dBFS > region_max_dbfs:
                        region_max_dbfs = chunk.dBFS
                        region_max_index = i
            else:
                # End of a region: record the peak timestamp and index if a region was active
                if region_start is not None:
                    timestamps.append(region_max_index * self.chunk_size_ms / 1000.0)
                    self.peak_indices.append(region_max_index)
                    region_start = None
                    region_max_dbfs = -float('inf')
                    region_max_index = None

        # Handle case where the audio ends while still in a region above the threshold
        if region_start is not None:
            timestamps.append(region_max_index * self.chunk_size_ms / 1000.0)
            self.peak_indices.append(region_max_index)
        
        self.timestamps = timestamps
        return self.timestamps

    def plot_results(self):
        if not self.chunks:
            return
        
        # Create arrays for time (in seconds) and loudness values for each chunk
        loudness = [chunk.dBFS for chunk in self.chunks]
        times = np.arange(len(loudness)) * (self.chunk_size_ms / 1000.0)
        
        plt.figure(figsize=(12, 6))
        plt.plot(times, loudness, label='Loudness (dBFS)')
        plt.axhline(y=self.threshold_dbfs, color='r', linestyle='--', label='Threshold')
        
        # Plot green dots at the peak positions
        if self.peak_indices:
            peak_times = [idx * self.chunk_size_ms / 1000.0 for idx in self.peak_indices]
            peak_loudness = [self.chunks[idx].dBFS for idx in self.peak_indices]
            plt.scatter(peak_times, peak_loudness, color='green', s=100, zorder=5, label='Peaks')
        
        plt.xlabel('Time (seconds)')
        plt.ylabel('Loudness (dBFS)')
        plt.title('Audio Loudness Analysis')
        plt.grid(True)
        plt.legend()
        plt.show()


def main():
    # Configure analysis parameters
    audio_path = "./eyeOfTigerOnsetSound.wav"
    analyzer = AudioAnalyzer(
        file_path=audio_path,
        chunk_size_ms=100,
        threshold_dbfs=-14.5
    )
    
    # Process audio: load, analyze, save peak timestamps, and plot results
    if analyzer.load_audio():
        timestamps = analyzer.analyze()
        print(f"Audio duration: {len(analyzer.audio) / 1000.0:.2f} seconds")
        print(f"Sample rate: {analyzer.audio.frame_rate} Hz")
        print("\nTimestamps where loudness exceeds threshold:")
        print(timestamps)
        
        # Save detected onsets to a text file
        with open("onsets.txt", "w") as f:
            for t in timestamps:
                f.write(f"{t}\n")
                
        analyzer.plot_results()
    
if __name__ == "__main__":
    main()
