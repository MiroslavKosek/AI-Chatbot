#!/usr/bin/python3

## This script recursively scrapes all URLs starting with the base URL and saves them to "links.txt".

import requests
from bs4 import BeautifulSoup
from urllib.parse import urljoin
import time
import os

def load_hrefs_from_file(filename):
    try:
        with open(filename, 'r') as file:
            return set(line.strip() for line in file.readlines())
    except FileNotFoundError:
        return set()

def save_hrefs_to_file(filename, hrefs):
    with open(filename, 'a') as file:
        for href in hrefs:
            file.write(f"{href}\n")

def scrape_all_hrefs(url, base_url):
    try:
        response = requests.get(url, timeout=10)
        response.raise_for_status()
        soup = BeautifulSoup(response.text, 'html.parser')
        
        hrefs = set()
        
        for a_tag in soup.find_all('a', href=True):
            full_url = urljoin(base_url, a_tag['href'])
            if full_url.startswith(base_url):
                hrefs.add(full_url)
        
        return hrefs
    except Exception as e:
        print(f"Error scraping {url}: {e}")
        return set()

def main():
    base_url = '' #insert base url here
    input_filename = 'links.txt'
    
    script_dir = os.path.dirname(os.path.abspath(__file__))
    input_filename = os.path.join(script_dir, 'links.txt')
    
    print(f"Saving links to: {input_filename}")
    
    visited_hrefs = load_hrefs_from_file(input_filename)
    
    if not visited_hrefs:
        print(f"No URLs found in {input_filename}. Adding base URL as starting point.")
        visited_hrefs.add(base_url)
        save_hrefs_to_file(input_filename, {base_url})
    
    urls_to_process = visited_hrefs.copy()
    
    new_hrefs_count = 0
    
    for url in urls_to_process:
        print(f"Scraping {url}")
        
        found_hrefs = scrape_all_hrefs(url, base_url)
        
        new_hrefs = found_hrefs - visited_hrefs
        
        if new_hrefs:
            print(f"Found {len(new_hrefs)} new URLs")
            new_hrefs_count += len(new_hrefs)
            
            save_hrefs_to_file(input_filename, new_hrefs)
            
            visited_hrefs.update(new_hrefs)
        
        time.sleep(1)
    
    print(f"Scraping finished! Found {new_hrefs_count} new URLs in total.")

if __name__ == '__main__':
    main()