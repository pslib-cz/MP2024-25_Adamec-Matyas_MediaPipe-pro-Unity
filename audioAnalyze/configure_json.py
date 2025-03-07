import json
import random

def create_level_from_onsets():
    # Hard-coded variables
    onsets_file = "onsets.txt"          # Input file
    output_file = "MyLevel.json"        # Output file
    level_id = 1                        # Level ID
    level_name = "My Level"             # Level name
    level_description = "my desc"       # Description
    TOTAL_TIME = 60                     # Level duration in seconds

    # Read the onsets.txt file
    with open(onsets_file, 'r', encoding='utf-8') as f:
        lines = [line.strip() for line in f if line.strip()]


    # Parse each line, create a Block for each time
    blocks = []

    for i, line in enumerate(lines):
        try:
            time_value = float(line)
        except ValueError:
            print(f"Skipping line {i+1}: Unable to parse time from '{line}'")
            continue

        # Randomly determine if block is on the right side
        is_right = random.choice([True, False])

        # If block isRight, x in [0, 1], otherwise x in [-1, 0]
        if is_right:
            x_pos = random.uniform(0.0, 1.0)
        else:
            x_pos = random.uniform(-1.0, 0.0)

        # y can remain in the full [-1, 1] range
        y_pos = random.uniform(-1.0, 1.0)

        block = {
            "id": i + 1,
            "time": time_value,
            "position": {
                "x": round(x_pos, 3),
                "y": round(y_pos, 3)
            },
            "type": "standard",
            "isRight": is_right,
            "points": 100
        }
        blocks.append(block)

    level = {
        "id": level_id,
        "name": level_name,
        "description": level_description,
        "Time": TOTAL_TIME,
        "blocks": blocks
    }

    # Write the Level object to MyLevel.json
    with open(output_file, 'w', encoding='utf-8') as f:
        json.dump(level, f, indent=4)

    print(f"Level JSON created and saved to '{output_file}'.")


if __name__ == "__main__":
    create_level_from_onsets()