Create database RaceDay;
Use RaceDay;

--User Login table--
CREATE TABLE Users(
UserID int IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(50) NOT NULL,
Email VARCHAR(20) UNIQUE NOT NULL, --no two user must share a email--
Password VARCHAR(20) NOT NULL,
ContactInfo VARCHAR(10),
UserType VARCHAR(50) NOT NULL,
CHECK (UserType IN ('Participant', 'Organizer')) --users can only be participants or organizers--
)

--participants table--
CREATE TABLE Participants(
ParticipantId int IDENTITY(1,1) PRIMARY KEY,
DateOfBirth VARCHAR(20) NOT NULL,
Gender VARCHAR(20) NOT NULL,
EmergencyContact VARCHAR(20) 
TeamName VARCHAR(20)
)

--organizers table--
--organizers can have multiple events/races--
CREATE TABLE Organizers(
OrganizerID int IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(20) UNIQUE NOT NULL,
Contact_Number(10) NOT NULL
)

--races table--
--participants can enter many races--
--races have types, long distance, relay etc.--
CREATE TABLE Races(
RaceID int IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(50) UNIQUE NOT NULL,
Location VARCHAR(50) NOT NULL,
StartTime VARCHAR(50) NOT NULL,
EndTime VARCHAR(50) NOT NULL,
Distance int NOT NULL,
Type VARCHAR(20) NOT NULL
)

--registrations table--
--participants can register for many race-- 
CREATE TABLE Registrations(
RegistrationID int IDENTITY(1,1) PRIMARY KEY,
BibNumber int ,
Status VARCHAR(20),
RegistrationDate DATE,
ParticipantID int,
RaceID int 

  --foreign keys to registration table--
FOREIGN KEY (ParticipantID)
REFERENCES Participants(ParticipantID),

FOREIGN KEY(RaceID)
REFERENCES Races(RaceID)
)

  --categories--
CREATE TABLE Categories(
CategoryID int IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(20) NOT NULL,
Description VARCHAR(50) NOT NULL
)

  --linking races and categories table--
CREATE TABLE RaceCategories(
CategoryID int,
RaceId int,

FOREIGN KEY (CategoryID)
REFERENCES Categories(CategoryID),

FOREIGN KEY (RaceID)
REFERENCES Races(RaceID)
)

--results table to view and record participant results--
CREATE TABLE Results(
ResultID int IDENTITY(1,1) PRIMARY KEY,
RegistrationID int,
FinishTime VARCHAR(50),
Position int,
Status VARCHAR(20),

FOREIGN KEY (RegistrationID)
REFERENCES Registrations(RegistrationID)
)

INSERT INTO Participants
VALUES ( '06/11/2004', 'Male', '0836627890', 'Red Tigers' ),
	   ( '08/12/2005', 'Female', NULL, 'Blue dragons' )




