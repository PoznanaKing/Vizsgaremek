-- phpMyAdmin SQL Dump
-- version 5.1.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Feb 12. 13:53
-- Kiszolgáló verziója: 10.4.20-MariaDB
-- PHP verzió: 7.3.29

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `auth`
--
CREATE DATABASE IF NOT EXISTS `auth` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `auth`;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `aspnetroleclaims`
--

CREATE TABLE `aspnetroleclaims` (
  `Id` int(11) NOT NULL,
  `RoleId` varchar(255) NOT NULL,
  `ClaimType` longtext DEFAULT NULL,
  `ClaimValue` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `aspnetroles`
--

CREATE TABLE `aspnetroles` (
  `Id` varchar(255) NOT NULL,
  `Name` varchar(256) DEFAULT NULL,
  `NormalizedName` varchar(256) DEFAULT NULL,
  `ConcurrencyStamp` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `aspnetroles`
--

INSERT INTO `aspnetroles` (`Id`, `Name`, `NormalizedName`, `ConcurrencyStamp`) VALUES
('3c16e158-8332-45f2-b9a7-4ab255ad8a47', 'Teacher', 'TEACHER', NULL),
('aae12ffc-9dee-490c-8c5e-5e4522287a8d', 'User', 'USER', NULL);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `aspnetuserclaims`
--

CREATE TABLE `aspnetuserclaims` (
  `Id` int(11) NOT NULL,
  `UserId` varchar(255) NOT NULL,
  `ClaimType` longtext DEFAULT NULL,
  `ClaimValue` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `aspnetuserlogins`
--

CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(255) NOT NULL,
  `ProviderKey` varchar(255) NOT NULL,
  `ProviderDisplayName` longtext DEFAULT NULL,
  `UserId` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `aspnetuserroles`
--

CREATE TABLE `aspnetuserroles` (
  `UserId` varchar(255) NOT NULL,
  `RoleId` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `aspnetuserroles`
--

INSERT INTO `aspnetuserroles` (`UserId`, `RoleId`) VALUES
('3266a698-4246-4aa2-906c-cc2b5673aa74', '3c16e158-8332-45f2-b9a7-4ab255ad8a47'),
('3266a698-4246-4aa2-906c-cc2b5673aa74', 'aae12ffc-9dee-490c-8c5e-5e4522287a8d'),
('a1453f56-89fc-4394-9184-534a3beb3e7a', '3c16e158-8332-45f2-b9a7-4ab255ad8a47');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `aspnetusers`
--

CREATE TABLE `aspnetusers` (
  `Id` varchar(255) NOT NULL,
  `FullName` longtext NOT NULL,
  `Age` int(11) NOT NULL,
  `UserName` varchar(256) DEFAULT NULL,
  `NormalizedUserName` varchar(256) DEFAULT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `NormalizedEmail` varchar(256) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext DEFAULT NULL,
  `SecurityStamp` longtext DEFAULT NULL,
  `ConcurrencyStamp` longtext DEFAULT NULL,
  `PhoneNumber` longtext DEFAULT NULL,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `aspnetusers`
--

INSERT INTO `aspnetusers` (`Id`, `FullName`, `Age`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail`, `EmailConfirmed`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`, `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`) VALUES
('3266a698-4246-4aa2-906c-cc2b5673aa74', 'Pozsgai Nándor', 0, 'SziaGyerek11', 'SZIAGYEREK11', 'pozsgain@kkszki.hu', 'POZSGAIN@KKSZKI.HU', 0, 'AQAAAAIAAYagAAAAEPVUr86ssDoWMuJP4XSpxsBgwfZWsewfGv50FzDYz2aj+ck2n5XjAk+p04VmEdQSjw==', 'UMRJFK2RZJ6BVRUSQ6T4XSN5OZYRBWD2', '1692fc36-873f-4437-b91c-d2a57cc9f499', NULL, 0, 0, NULL, 1, 0),
('6639c31b-b55d-420a-912f-b6e5ab3d643b', 'Teszter Ember', 0, 'Tesztfiok', 'TESZTFIOK', 'pozsgain@kkszki.hu', 'POZSGAIN@KKSZKI.HU', 0, 'AQAAAAIAAYagAAAAELFxuhrBIyXm9UKwv1FW++toGSP8OCi/hbAB/mcXtIvWHIC8dVmED/vU9RUDMHlwGg==', 'R4HQ76LWTULM5RSP5BUNTSXFTZYMCQMV', '320ece91-ad1e-416b-a356-544a5aeeb46f', NULL, 0, 0, NULL, 1, 0),
('71447213-dc90-4e35-a5ef-d05db3c48420', 'Szia Uram', 0, 'SziaUram11', 'SZIAURAM11', 'pozsgain@kkszki.hu', 'POZSGAIN@KKSZKI.HU', 0, 'AQAAAAIAAYagAAAAEFdRtPnfWNeVdzRwjhNycWRLWe8pvZvgqTxSYG2rwZ3ARRXSIDFWYQH/6IUNHNhDlA==', 'XQR5XJZKXB52NEGUYDXKRPXYYCTNGKLU', 'bc6a403c-09c8-4062-adad-d0235b569631', NULL, 0, 0, NULL, 1, 0),
('a1453f56-89fc-4394-9184-534a3beb3e7a', 'ASd ASD', 0, 'Asdasd11', 'ASDASD11', 'pozsgain@kkszki.hu', 'POZSGAIN@KKSZKI.HU', 0, 'AQAAAAIAAYagAAAAEEn57+Zn1Rt4Q7P0Q4eTGQQrC0acTHJn4Buudj0JlqU8+HNygBosIyMjQwAM1ydG1Q==', '46TOAR2FYTTAQKRF3XMI7DEF6WHEUUFD', '1fab102f-6eb8-43e3-ab83-9435975c9636', NULL, 0, 0, NULL, 1, 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `aspnetusertokens`
--

CREATE TABLE `aspnetusertokens` (
  `UserId` varchar(255) NOT NULL,
  `LoginProvider` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Value` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `comments`
--

CREATE TABLE `comments` (
  `commentid` int(11) NOT NULL,
  `commentername` text NOT NULL,
  `commentcontent` text NOT NULL,
  `postid` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `comments`
--

INSERT INTO `comments` (`commentid`, `commentername`, `commentcontent`, `postid`) VALUES
(3, 'SziaGyerek11', 'SZIAAAAAAAA', 2);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `places`
--

CREATE TABLE `places` (
  `placeid` int(11) NOT NULL,
  `placename` text NOT NULL,
  `postalcode` int(11) NOT NULL,
  `townname` text NOT NULL,
  `streetname` text NOT NULL,
  `storylevel` int(11) DEFAULT NULL,
  `description` text NOT NULL,
  `rating` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `posttable`
--

CREATE TABLE `posttable` (
  `postid` int(11) NOT NULL,
  `posttitle` text NOT NULL,
  `postimage` longblob DEFAULT NULL,
  `postdescription` text NOT NULL,
  `userid` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `posttable`
--

INSERT INTO `posttable` (`postid`, `posttitle`, `postimage`, `postdescription`, `userid`) VALUES
(2, 'Tesztelés', 0xffd8ffe000104a46494600010100000100010000ffdb0084000906070f07061512070f1515121716101115171512101516161216151718151b17181b2834201f1a311c1a1a213322252a2b322e2e171f3338352c37282d3a2c010a0a0a0e0d0e1a101018371d1d252d2e2d2d2d2f2e372d2d2d2d2d2d2d2d2d2d2d2d2d2f2d2d2d2d2d2d2d2d2b2d2d2b2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2bffc000110800a8012b03011100021101031101ffc4001b00010003010101010000000000000000000005060704030102ffc40045100002010104040a050905090000000000000102030405061112213163071316224151719193e1617281b1b2142332527392a1a2c136425362c2152633354482a3d1d2ffc400190101000301010000000000000000000000000304050201ffc40029110100020102050304030100000000000000010203041113142131f01232712233418151526123ffda000c03010002110311003f009b2e3180000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000008ebf2f9a37258f8cb5bdbaa315f4a72ea5ff7d0737b4563aa5c58ad9276867f531fdb656bd2a71a6a1f51c5b597a65b73f4eaec2b71adbb423458f6dbf2b7e1dc5d67be5a84fe6eb7d4935949ff0024ba7b35326a658b29e6d2db1f5ef0b112ab0000000000000000000000000000000000000000000000014ec418ea958a6e175c5559ad4e4dfcda7ecd72f664bd2437cd11d2177168e6dd6dd14db6e29b75b64f8cb4ca2baa9fcda5f775f7b641392d3f95dae9f1d7b43c28e20b6d1ff0ed757db394be23c8bda3f2ea70e39ef574c716de31d96a97b6149fbe27bc5bff002e396c5fd5fae585e2ff00d53f0e87fe4716ff00c9cb62fe1e6f15de12db6a9fb1535ee438b6fe5ef2f8bfabca588edd2db6babec9b5ee3ce25bf97bc0c7fd5c56bb655b6d452b6559cdad49ce52934b6eacce66667ba4ad6b5e911b3c0f1e9d205b2e9c796ab1a8c6db18d5824966f38d4cbd6d8df6ad7d64d5cd31dfaaa64d1d2dd6bd25a1dd17a51be2c6aa58a59c7634f54a32e9525d0cb35b45a3786764c76a4ed6769d2300000000000000000000000000000000000000000067b8ff00123955764b0cb28ad55a4ba5fd44fabafaf675e75b2e4fc43474983a7aedfa514aebe0000000000000000026709df2ee5bde3293f9a9e50aaba3473d52ed4de7d99f59263b7a650ea317129b7e5b19758a000000000000000000000000000000000000000040629c4d4ee2a1a31e757926e115d1d529f52cfa3a72edca3c9922ab18304e49dff0c8e5273937379b6db6ded6deb6d949b111b767c000000000000000000001b160eb73b7e1ca529bce497152ebce0f4737ec49fb4bd8e77ab1b534f4e4984d1da00000000000000000000000000000000000000e6bc6d6ac177d4ab5166a1094f2ebc96797e87969da37754afaad156456cbbed96eb1cadd6986942726e52cd75e8e7a3b7433e6aec295a2d3f5366b7a567871dd127094000000000000000007ad9a84ad569853a39694e50a71cda4b4a525159b7b166f69e89bc5b836dd846bc637bc23a33fa1529c9ca9c9f4c749a4d4bd0d2f46626367916dd66e0bad1a577d6a79fd1a919fb271cbdf0659c13d36676babf544aec4ea200000000000000000000000000000000000015be10ad1c4616a993fa52a70fcca4ff08b22cd3f4ad68e37cb0e8a9763960cf93d35cef93a82f5d4335f98f7d3f46ce6327fdbd5feb1f4f34526c0000000000000024b0e5dd0bdaf8850af29454d54d71cb34e309496de8d4774afaa76479b24d29ea85c2c3c1efc9ef084ebda14e9c6519b8f14d3964f349eb6b2eb268c1b4f753beb77aed108bc6d857fb3a4ebd823f30df3a3fc36ff00a3ddb366473971edd61269b51eb8f45bbb41c0f89e8e27b9d5d18e63f393a70e2673793ad4dace9bd27b2b2e87d3975e79f1f29b789eb0e6b93005b709df55d68bab669462e9d58e4dbd193c94e0b5a96527d196ada498a76957d5c4dab1b25b63d65966800000000000000000000000000000000000053784c7c65df429fd7ac97e56bfa88737685dd147d569ff0016db4cf88b2ca4bf76327dc9b259e90a95eb6860f1fa3accf6f3e800000000000009ac172d1c5343d692efa734498bdd087531ff002b3622eb15f9a905520e351269a69a6b34d3da9aea0f6276517850b125468d682c9a6e8bcbab27287734fbcaf9e3b4afe86fd66b3f2efe0cf1cde73bc9d0b4daa55694694a51555466d38ca0973df3dea6f6b38c51ea9da536a6de8aef0bede57bcef18fcfc29a7f5a30ca5d99be82cd69b33af966fde11e7488000000000000000000000000000000000014ac76dd5bfac14d74d5cf2eda9492f7320cbde217b4bd297959efda9c55c95e5d546bbee84896fda557147d71f2c456c283740f00003601d162b156b7d4d1b0d29547b39b16d2ed7b17b4f62b33d9cdaf5af79d93b6dc1d5aeeb8e768b7ce2a51d0ca9c79cf9d38c79d2d9d3b167da4938a62bbca0aea6b7bc56aad112c804ae1479625b3fda47dccef1fba1167fb76f86ce5e620054784cff208fdb43e199067f6ae68bee7e95ee0cd7f7865f6353e3a64783dcb3adfb71f2d3cb6ca000000000000000000000000000000000000088b45cff2ac490b4d76b46953d1a71d79f18dcb393e8cb26b2f4f61c4d77b6e9a32ed8e691f97dc573e2f0d5a1eea6bbd65fa8c9ed934f1be4afcb1928b6800936f9ab37b12eb7d006a960c0d62a5678fcae9b9d4d18e9b752a24e5973b527965997230d76eacabeaf24cced3d12b67c3d62b33ce8d96926b63708c9f7cb59d452b1f8436cf92ddec928c546394564ba96c3b47ba131b2cf0ad7f560fbaa4591e5f64a7d37ddab1f29361d778d8fe46e9ff003d1a35befa6cead5d9cd2deadfe7675e12fda6b3e7fc45ee67b8fdd0e33fdbb7c3662f31002a5c267ecfc7eda1f0cc833fb57345f73f481e0c239df551f5527f8ce27183dcb1adf647cb4b2d32c000000000000000000000000000000000000005771fd6e2b0b54fe674a1df38b7f826459bdab3a48df2c327a74e556a28d259ca4d462974c9bc92ef2a4756bcced1bcadf8bb0c46e8b8684e8eb9c5e85592fde73d79f6292d15e8689b263f4d614f4fa89c992627f4af61ea5c75fd422f63ad4bb9493fd08a9ee858cb3b52d3fe36c2fb0c00042e3379616af9fd44bbe51488f2fb653e9beed58e4b52293696ac7566541591c7a6cd4e9fdcc9af889b2c6db7c2a696dbfabe50d876a7157fd9defa92ef925fa91d3dd09b2c6f8ed1fe36c2fb0c02adc23c34b0d36ba2a527ef8fea459bdab7a39ffa7e903c16c73bcab3eaa705df2f223c1de5635d3f4c47fad1cb2cc00000000000000000000000000000000000000143e14ad8d52a3463b1b9d597fb728c7e297715f3cf6868686bdecad60ba5c7629a09ec5294beec24d7e291162f742d6a676c52bdf088ff00baf3f5e8fc68b19bdacfd1fdd66b735a958af7a352a7d1854a7297aaa4b4bf0ccab59dad12d3c95f55261b7a79ad45f613e8002bd8fa5a3852afa5d15ff2c08f37b256749f76191cfe8bec6526c2f5c2451cac7649754670fcb4dfe858cdda143473d6d0a5d96af116a84feace12fbb24ff4208eebb68de25bbbdba8d06081e23b10d83fb4ee5ab496d945e8faf1e743f3247378deb30970dfd178952782da995e15a2f6ba7097dd93cfe220c1de5775d1f4d65a39659a000000000000000000055797d60def87e645c6aadf2593c9397d60def87e638d5392c9e49cbeb06f7c3f31c6a9c964f24e5f5837be1f98e354e4b279272fac1bdf0fcc71aa72593c9397d60def87e638d5392c9e49cbeb06f7c3f31c6a9c964f24e5f5837be1f98e354e4b279272fac1bdf0fcc71aa72593c9397d60def87e638d5392c9e49cbeb06f7c3f31c6a9c96453319df34efabd633b1e968469c61ce593d2d2937abda8af96f169e8bba6c538ebb4b930cde10baafca75ad19e8474f4b259bc9c24b52ed68f296f4db7779a937a4d6166c598aec97bdc92a565e334dca9c9694325cd926f5e7d5992e4c95b576856d3e9af8efea9514aebcbd616c6b4ac3762a57ae9b70e6c2518e9670e84fd2b6766459c79a2236950cfa49b5bd544c72fac1bdf0fccef8d543c964f24e5f5837be1f98e354e4b279288c578b6cb7b5c73a364d3d36e9b5a50c96519a6f5e7d48e326589aed09b069af4bfaa54392ce3a8acbeb7e32c4566be6eda50b1e9e9c269bd2864b2d069e4fb7226c978b446ca9a7c16c76999fcaa1259c4856da850c7b628d08aa9c6e968c73ca9f4e5afa4b719abb32e747937e8fdf2fac1bdf0fccf78d579c964f24e5fd837be1f98e354e4b2792a8dcb7d50baf1654af0d2e227c725cde728cde92d5eb24bb082b788beff0085cc98ad7c515fcaddcbeb06f7c3f327e3554f92c9e49cbeb06f7c3f31c6a9c964f24e5f5837be1f98e354e4b279272fac1bdf0fcc71aa72593c9397d60def87e638d5392c9e49cbeb06f7c3f31c6a9c964f24e5f5837be1f98e354e4b279272fac1bdf0fcc71aa72593c9397d60def87e638d5392c9e49cbeb06f7c3f31c6a9c964f24e5f5837be1f98e354e4b279272fac1bdf0fcc71aa72593c965a536a8000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007fffd9, 'Ez is egy teszt', '3266a698-4246-4aa2-906c-cc2b5673aa74');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `userplaceconnector`
--

CREATE TABLE `userplaceconnector` (
  `userid` varchar(255) NOT NULL,
  `placeid` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20250113073558_CreateAuthDb', '8.0.11');

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `aspnetroleclaims`
--
ALTER TABLE `aspnetroleclaims`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`);

--
-- A tábla indexei `aspnetroles`
--
ALTER TABLE `aspnetroles`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `RoleNameIndex` (`NormalizedName`);

--
-- A tábla indexei `aspnetuserclaims`
--
ALTER TABLE `aspnetuserclaims`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_AspNetUserClaims_UserId` (`UserId`);

--
-- A tábla indexei `aspnetuserlogins`
--
ALTER TABLE `aspnetuserlogins`
  ADD PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  ADD KEY `IX_AspNetUserLogins_UserId` (`UserId`);

--
-- A tábla indexei `aspnetuserroles`
--
ALTER TABLE `aspnetuserroles`
  ADD PRIMARY KEY (`UserId`,`RoleId`),
  ADD KEY `IX_AspNetUserRoles_RoleId` (`RoleId`);

--
-- A tábla indexei `aspnetusers`
--
ALTER TABLE `aspnetusers`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  ADD KEY `EmailIndex` (`NormalizedEmail`);

--
-- A tábla indexei `aspnetusertokens`
--
ALTER TABLE `aspnetusertokens`
  ADD PRIMARY KEY (`UserId`,`LoginProvider`,`Name`);

--
-- A tábla indexei `comments`
--
ALTER TABLE `comments`
  ADD PRIMARY KEY (`commentid`),
  ADD KEY `post_id` (`postid`);

--
-- A tábla indexei `places`
--
ALTER TABLE `places`
  ADD PRIMARY KEY (`placeid`);

--
-- A tábla indexei `posttable`
--
ALTER TABLE `posttable`
  ADD PRIMARY KEY (`postid`),
  ADD KEY `user_id` (`userid`);

--
-- A tábla indexei `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `aspnetroleclaims`
--
ALTER TABLE `aspnetroleclaims`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `aspnetuserclaims`
--
ALTER TABLE `aspnetuserclaims`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `comments`
--
ALTER TABLE `comments`
  MODIFY `commentid` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT a táblához `places`
--
ALTER TABLE `places`
  MODIFY `placeid` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8320858;

--
-- AUTO_INCREMENT a táblához `posttable`
--
ALTER TABLE `posttable`
  MODIFY `postid` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `aspnetroleclaims`
--
ALTER TABLE `aspnetroleclaims`
  ADD CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE;

--
-- Megkötések a táblához `aspnetuserclaims`
--
ALTER TABLE `aspnetuserclaims`
  ADD CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;

--
-- Megkötések a táblához `aspnetuserlogins`
--
ALTER TABLE `aspnetuserlogins`
  ADD CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;

--
-- Megkötések a táblához `aspnetuserroles`
--
ALTER TABLE `aspnetuserroles`
  ADD CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;

--
-- Megkötések a táblához `aspnetusertokens`
--
ALTER TABLE `aspnetusertokens`
  ADD CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;

--
-- Megkötések a táblához `comments`
--
ALTER TABLE `comments`
  ADD CONSTRAINT `comments_ibfk_1` FOREIGN KEY (`postid`) REFERENCES `posttable` (`postid`) ON DELETE CASCADE;

--
-- Megkötések a táblához `posttable`
--
ALTER TABLE `posttable`
  ADD CONSTRAINT `posttable_ibfk_1` FOREIGN KEY (`userid`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
