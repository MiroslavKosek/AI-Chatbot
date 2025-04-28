#!/usr/bin/python3

## This script reads URLs from links.txt, downloads each webpage, cleans the text content, and saves it into separate .txt files inside the 'scraped_texts' folder.

import requests
from bs4 import BeautifulSoup
import os
import re
from urllib.parse import urlparse

def sanitize_filename(url):
    parsed = urlparse(url)
    base = parsed.netloc + parsed.path
    base = re.sub(r'[^\w\-_\. ]', '_', base)
    if len(base) > 100:
        base = base[:100]
    return base.strip('_') + '.txt'

def scrape_and_save(url, output_dir):
    try:
        response = requests.get(url)
        response.raise_for_status()
        soup = BeautifulSoup(response.text, 'html.parser')

        for tag in soup(['script', 'style', 'noscript', 'a']):
            tag.decompose()

        text = soup.get_text(separator='\n')
        lines = [line.strip() for line in text.splitlines()]
        non_empty_lines = [line for line in lines if line]
        
        cleaned_text = f"{url}\n\n" + '\n'.join(non_empty_lines)

        filename = sanitize_filename(url)
        filepath = os.path.join(output_dir, filename)

        with open(filepath, 'w', encoding='utf-8') as file:
            file.write(cleaned_text)

        print(f"Saved: {filepath}")
        return True
    except Exception as e:
        print(f"Error in processing {url}: {str(e)}")
        return False

def process_links_from_file():
    script_dir = os.path.dirname(os.path.abspath(__file__))
    
    output_dir = os.path.join(script_dir, 'scraped_texts')
    os.makedirs(output_dir, exist_ok=True)
    
    links_file = os.path.join(script_dir, 'links.txt')
    
    try:
        with open(links_file, 'r', encoding='utf-8') as file:
            urls = [line.strip() for line in file if line.strip()]
        print(f"Reading URLs from: {links_file}")
    except FileNotFoundError:
        print(f"The links file was not found: {links_file}")
        return

    print(f"Found {len(urls)} URLs to process")
    print(f"Output directory: {output_dir}")
    
    for url in urls:
        if url:
            scrape_and_save(url, output_dir)

if __name__ == '__main__':
    process_links_from_file()