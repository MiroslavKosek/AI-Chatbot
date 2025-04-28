/** @type {import('next').NextConfig} */
const nextConfig = {
  allowedDevOrigins: [
    "http://localhost:3000", 
    "http://10.38.144.77:3000", 
  ],
  reactStrictMode: true,
}

module.exports = nextConfig
