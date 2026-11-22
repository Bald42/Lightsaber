using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

namespace Lightsaber.Network.Utility
{
    public static class NetworkCodeUtility
    {
        private const string ALPHABET = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";
        private const string DEFAULT_FALLBACK_IP = "127.0.0.1";
        private const string DEFAULT_PIN_FALLBACK = "AAAA";

        /// <summary>
        /// Достаёт локальный IP реального адаптера. Раньше тут был жёсткий фильтр по типу
        /// (только Ethernet/Wireless80211) — на части Android-телефонов реальный Wi-Fi
        /// репортится другим типом (Unknown и т.п.), фильтр резал его и всё падало на
        /// заглушку 127.0.0.1. Поэтому тип не проверяем вообще, только отсекаем loopback,
        /// туннели и явно VPN-образные адаптеры по описанию (как было до прошлого фикса).
        /// Среди оставшихся предпочитаем адаптер с настоящим шлюзом (не 0.0.0.0) — у VPN/
        /// прокси-туннелей (точка-точка) шлюза обычно нет, в отличие от обычной домашней сети.
        /// Если шлюз нигде не отдаётся (бывает на Android), откатываемся на первый
        /// подходящий адаптер вообще.
        /// </summary>
        public static string GetLocalIPAddress()
        {
            try
            {
                string fallbackCandidate = null;

                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus != OperationalStatus.Up ||
                        ni.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                        ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
                        continue;

                    string description = ni.Description.ToLower();
                    if (description.Contains("virtual") || description.Contains("tap") ||
                        description.Contains("vpn") || description.Contains("zerotier") ||
                        description.Contains("hamachi") || description.Contains("veth"))
                        continue;

                    IPInterfaceProperties ipProps = ni.GetIPProperties();

                    string ipv4 = null;
                    foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipv4 = ip.Address.ToString();
                            break;
                        }
                    }

                    if (ipv4 == null)
                        continue;

                    bool hasRealGateway = false;
                    foreach (GatewayIPAddressInformation gateway in ipProps.GatewayAddresses)
                    {
                        if (gateway.Address.AddressFamily == AddressFamily.InterNetwork &&
                            gateway.Address.ToString() != "0.0.0.0")
                        {
                            hasRealGateway = true;
                            break;
                        }
                    }

                    if (hasRealGateway)
                    {
                        return ipv4;
                    }

                    fallbackCandidate ??= ipv4;
                }

                if (fallbackCandidate != null)
                {
                    return fallbackCandidate;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetworkCodeUtility] Ошибка при поиске IP: {e.Message}");
            }

            return DEFAULT_FALLBACK_IP;
        }

        /// <summary>
        /// Кодирует последние 2 октета IP (parts[2] и parts[3]) в 4-значный PIN
        /// </summary>
        public static string IpToPin(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress)) return DEFAULT_PIN_FALLBACK;

            string[] parts = ipAddress.Split('.');
            if (parts.Length != 4) return DEFAULT_PIN_FALLBACK;

            if (!int.TryParse(parts[2], out int byte3) || !int.TryParse(parts[3], out int byte4))
                return DEFAULT_PIN_FALLBACK;

            int combined = (byte3 << 8) | byte4;

            char[] code = new char[4];
            for (int i = 3; i >= 0; i--)
            {
                code[i] = ALPHABET[combined % ALPHABET.Length];
                combined /= ALPHABET.Length;
            }

            return new string(code);
        }

        /// <summary>
        /// Преобразует PIN обратно в IP, восстанавливая последние 2 октета.
        /// prefixOrSubnet может передаваться как "192.168", "192.168.", или "192.168.1."
        /// </summary>
        public static string PinToIp(string pin, string prefixOrSubnet = "192.168")
        {
            if (string.IsNullOrEmpty(pin)) return null;
            
            pin = pin.ToUpper().Trim();
            if (pin.Length != 4) return null;

            int combined = 0;
            foreach (char c in pin)
            {
                int index = ALPHABET.IndexOf(c);
                if (index == -1) return null; // Невалидный символ
                combined = combined * ALPHABET.Length + index;
            }

            int byte3 = (combined >> 8) & 0xFF;
            int byte4 = combined & 0xFF;

            // Вычленяем только первые два октета (например, "192.168" из "192.168.1.")
            string[] parts = prefixOrSubnet.Split('.', StringSplitOptions.RemoveEmptyEntries);
            string basePrefix = parts.Length >= 2 ? $"{parts[0]}.{parts[1]}" : "192.168";

            return $"{basePrefix}.{byte3}.{byte4}";
        }
    }
}