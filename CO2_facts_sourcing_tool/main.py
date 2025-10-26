from google import genai
import ast
import requests
import json
import time

# The client gets the API key from the environment variable `GEMINI_API_KEY`.
client = genai.Client()

# The prompt expects a number of facts to be generated in string format.
NUMBER_OF_FACTS = "five" 

PROMPT_HEADER = f"Generate a list of {NUMBER_OF_FACTS} co2 emission facts."

PROMPT = PROMPT_HEADER + """
Your response must include nothing but the facts.
Do not include "[]" signs.
Do wrap each fact in "{}" signs.
Separate each fact with the following: <<ENDOFFACT>>.
You must make sure that the URLs are valid and that they do not return 404.
Each fact must contain the following information in the following order and format:
{
    "Source": A URL to where you sourced the fact from.
    "Fact": The fact itself in plaintext.
    "Co2": The total amount of Co2 emitted in grams. Do not include the "g".
    "Timespan": The number of seconds over which the Co2 is emitted. Do not include the "s".
}
Here is an example of a successful record:
{
    "Source": "https://www.mosspure.com/science-of-live-moss/",
    "Fact": "some moss can soak up 5.4 kg of co2eq a day",
    "Co2": "5400",
    "Timespan": "86400"
}
"""

response = client.models.generate_content(
    model="gemini-2.5-flash", contents=PROMPT
)
print("gemini response:")
print(response.text)

facts = response.text.split("<<ENDOFFACT>>")
for fact in facts:
    fact = fact.strip()
    if not fact:
        continue
    try:
        record = ast.literal_eval(fact)
    except Exception as e:
        print(f"Failed to parse fact: {fact}\nError: {e}")
        continue

    print(f"Submitting new record: {record}")
    payload = {
        "Source": record["Source"],
        "Fact": record["Fact"],
        "Co2": record["Co2"],
        "Co2Unit": "1",
        "Timespan": record["Timespan"],
        "TimespanUnit": "1"
    }
    
    try:
        r = requests.post(
            "https://www.blazejowski.co.uk/api/collaborations/co2_fact_submissions",
            headers={"Content-Type": "application/json","Connection": "Close"},
            data=json.dumps(payload),
            timeout=10
        )
        print(f"Status: {r.status_code}")
        print(f"Response: {r.text}")
        time.sleep(1)
    except requests.RequestException as e:
        print(f"Request failed: {e}")