using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TeamProjectServer.Models;

#nullable disable

namespace TeamProjectServer.Migrations
{
    /// <inheritdoc />
    public partial class reset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accessorys",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Element = table.Column<int>(type: "integer", nullable: false),
                    Grade = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Hp = table.Column<float>(type: "real", nullable: false),
                    HPPer = table.Column<float>(type: "real", nullable: false),
                    MPPer = table.Column<float>(type: "real", nullable: false),
                    GoldPer = table.Column<float>(type: "real", nullable: false),
                    IconKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accessorys", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "artifacts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Element = table.Column<int>(type: "integer", nullable: false),
                    Grade = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_artifacts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "playerAccountData",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    ATKPower = table.Column<float>(type: "real", nullable: false),
                    MaxHP = table.Column<float>(type: "real", nullable: false),
                    HPRegenPerSec = table.Column<float>(type: "real", nullable: false),
                    MaxMP = table.Column<float>(type: "real", nullable: false),
                    CriticalRate = table.Column<float>(type: "real", nullable: false),
                    CriticalDamage = table.Column<float>(type: "real", nullable: false),
                    MPRegenPerSec = table.Column<float>(type: "real", nullable: false),
                    GoldMultiplier = table.Column<float>(type: "real", nullable: false),
                    CurGold = table.Column<float>(type: "real", nullable: false),
                    EXPMultiplier = table.Column<float>(type: "real", nullable: false),
                    ATKSpeed = table.Column<float>(type: "real", nullable: false),
                    MoveSpeed = table.Column<float>(type: "real", nullable: false),
                    LastLoginTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Inventory = table.Column<List<InventorySlot>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_playerAccountData", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "playerInits",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    ATKPower = table.Column<float>(type: "real", nullable: false),
                    MaxHP = table.Column<float>(type: "real", nullable: false),
                    HPRegenPerSec = table.Column<float>(type: "real", nullable: false),
                    MaxMP = table.Column<float>(type: "real", nullable: false),
                    CriticalRate = table.Column<float>(type: "real", nullable: false),
                    CriticalDamage = table.Column<float>(type: "real", nullable: false),
                    MPRegenPerSec = table.Column<float>(type: "real", nullable: false),
                    GoldMultiplier = table.Column<float>(type: "real", nullable: false),
                    CurGold = table.Column<float>(type: "real", nullable: false),
                    EXPMultiplier = table.Column<float>(type: "real", nullable: false),
                    ATKSpeed = table.Column<float>(type: "real", nullable: false),
                    MoveSpeed = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_playerInits", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Elemnet = table.Column<int>(type: "integer", nullable: false),
                    Grade = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false),
                    Sound = table.Column<string>(type: "text", nullable: false),
                    Effect = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skills", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "stages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Element = table.Column<int>(type: "integer", nullable: false),
                    Grade = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stages", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Weapon",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Element = table.Column<int>(type: "integer", nullable: false),
                    Grade = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    EquipATK = table.Column<float>(type: "real", nullable: false),
                    PassiveATK = table.Column<float>(type: "real", nullable: false),
                    CriticalDMG = table.Column<float>(type: "real", nullable: false),
                    CriticalRate = table.Column<float>(type: "real", nullable: false),
                    GoldPer = table.Column<float>(type: "real", nullable: false),
                    IconKey = table.Column<string>(type: "text", nullable: false),
                    SoundKey = table.Column<string>(type: "text", nullable: false),
                    EffectKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapon", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_playerAccountData_Email",
                table: "playerAccountData",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accessorys");

            migrationBuilder.DropTable(
                name: "artifacts");

            migrationBuilder.DropTable(
                name: "playerAccountData");

            migrationBuilder.DropTable(
                name: "playerInits");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropTable(
                name: "stages");

            migrationBuilder.DropTable(
                name: "Weapon");
        }
    }
}
