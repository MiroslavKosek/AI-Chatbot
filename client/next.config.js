/** @type {import('next').NextConfig} */
const nextConfig = {
  allowedDevOrigins: [
    "http://localhost:3000", 
    "http://host.docker.internal:3000",
  ],
  reactStrictMode: true,
}

module.exports = nextConfig
