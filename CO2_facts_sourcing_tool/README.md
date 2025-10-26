# CO2 Facts Submission Bot

This script automatically generates a pre-defined number of CO2 emission facts and uploads them to [Co2 Facts Submissions API](https://www.blazejowski.co.uk/api/collaborations/co2_fact_submissions).

### Usage

###### All instructions provided here are for linux.

To use this script, first acquire a gemini API token. This can be done here: [Google AI Studio](https://aistudio.google.com/api-keys).
Once you have the token, export it as an environment variable with:

```bash
export GEMINI_API_KEY="token"
```

Then, build a new python virtual environment and install all required packages with:

```bash
python3 -m venv venv
venv/bin/python3 -m pip install -r requirements.txt
```

If this process succeeds, you're all set to run the script with either:

```python
venv/bin/python3 main.py
```

or

```bash
source venv/bin/activate
python3 main.py
```

Once the script runs, you can verify that the facts were added by navigating to: [Co2 Facts Submissions](https://www.blazejowski.co.uk/collaborations/co2_fact_submissions). 

##### Disclaimer:

The process is still prone to failure due to gemini's hallucinations. Sometimes gemini can randomly mess up the requested format or include random artifacts that make it impossible to parse the response. If that happens, simply run the script again - most of the time it *does* work.

### Known problems:

Gemini was trained on URLs that might by now be expired, which means that some of the responses *will* include URLs that return 404. This problem isn't specific to this bot as even if a URL is valid at the date of release of this application, they may still expire in the future. As of yet I am unaware of any plans to mitigate the issue, so our best recourse against this is simply trimming records with expired URLs in our view at [Co2 Facts Submissions](https://www.blazejowski.co.uk/collaborations/co2_fact_submissions).
