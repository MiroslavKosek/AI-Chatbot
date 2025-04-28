import axios, { AxiosRequestConfig } from 'axios';

// defaultní url k api
const instance = axios.create({
  baseURL: 'http://host.docker.internal:8080/ChatBot',
});

const defaultHeaders = {
  'Content-Type': 'application/json',
};


export const get = async (url: string, config?: AxiosRequestConfig<any>) => {
  const requestConfig = { headers: { ...defaultHeaders }, ...config };
  const response = await instance.get(url, requestConfig);
  return response;
};

export const post = async (url: string, data: any, config?: AxiosRequestConfig<any>) => {
  const requestConfig = { headers: { ...defaultHeaders }, ...config };
  const response = await instance.post(url, data, requestConfig);
  return response;
};

export const put = async (url: string, data: any, config?: AxiosRequestConfig<any>) => {
  const requestConfig = { headers: { ...defaultHeaders }, ...config };
  const response = await instance.put(url, data, requestConfig);
  return response;
};

export const del = async (url: string, config?: AxiosRequestConfig<any>) => {
  const requestConfig = { headers: { ...defaultHeaders }, ...config };
  const response = await instance.delete(url, requestConfig);
  return response;
};