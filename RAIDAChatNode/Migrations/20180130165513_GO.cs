using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace RAIDAChatNode.Migrations
{
    public partial class GO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberInGroup",
                columns: table => new
                {
                    memberId = table.Column<Guid>(nullable: false),
                    groupId = table.Column<Guid>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberInGroup", x => new { x.memberId, x.groupId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "Shares",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    current_fragment = table.Column<int>(nullable: false),
                    death_date = table.Column<long>(nullable: false),
                    file_data = table.Column<byte[]>(nullable: true),
                    file_extention = table.Column<string>(nullable: true),
                    kb_size = table.Column<int>(nullable: false),
                    ownerprivate_id = table.Column<Guid>(nullable: true),
                    sending_date = table.Column<long>(nullable: false),
                    to_groupgroup_id = table.Column<Guid>(nullable: true),
                    total_fragment = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shares", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    group_id = table.Column<Guid>(nullable: false),
                    group_name_part = table.Column<string>(nullable: true),
                    one_to_one = table.Column<bool>(nullable: false),
                    organizationprivate_id = table.Column<Guid>(nullable: true),
                    ownerprivate_id = table.Column<Guid>(nullable: true),
                    photo_fragment = table.Column<string>(nullable: true),
                    privated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.group_id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    private_id = table.Column<Guid>(nullable: false),
                    kb_of_credit = table.Column<int>(nullable: false),
                    org_name_part = table.Column<string>(nullable: true),
                    ownerprivate_id = table.Column<Guid>(nullable: true),
                    public_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.private_id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    private_id = table.Column<Guid>(nullable: false),
                    description_fragment = table.Column<string>(nullable: true),
                    kb_bandwidth_used = table.Column<int>(nullable: false),
                    last_use = table.Column<long>(nullable: false),
                    login = table.Column<string>(nullable: true),
                    nick_name = table.Column<string>(nullable: true),
                    online = table.Column<bool>(nullable: false),
                    organizationprivate_id = table.Column<Guid>(nullable: true),
                    pass = table.Column<string>(nullable: true),
                    photo_fragment = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.private_id);
                    table.ForeignKey(
                        name: "FK_Members_Organizations_organizationprivate_id",
                        column: x => x.organizationprivate_id,
                        principalTable: "Organizations",
                        principalColumn: "private_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ownerprivate_id = table.Column<Guid>(nullable: true),
                    rollbackTime = table.Column<long>(nullable: false),
                    tableName = table.Column<string>(nullable: true),
                    tableRowId = table.Column<string>(nullable: true),
                    transactionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Members_ownerprivate_id",
                        column: x => x.ownerprivate_id,
                        principalTable: "Members",
                        principalColumn: "private_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_organizationprivate_id",
                table: "Groups",
                column: "organizationprivate_id");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ownerprivate_id",
                table: "Groups",
                column: "ownerprivate_id");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInGroup_groupId",
                table: "MemberInGroup",
                column: "groupId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_organizationprivate_id",
                table: "Members",
                column: "organizationprivate_id");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_ownerprivate_id",
                table: "Organizations",
                column: "ownerprivate_id");

            migrationBuilder.CreateIndex(
                name: "IX_Shares_ownerprivate_id",
                table: "Shares",
                column: "ownerprivate_id");

            migrationBuilder.CreateIndex(
                name: "IX_Shares_to_groupgroup_id",
                table: "Shares",
                column: "to_groupgroup_id");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ownerprivate_id",
                table: "Transactions",
                column: "ownerprivate_id");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInGroup_Members_memberId",
                table: "MemberInGroup",
                column: "memberId",
                principalTable: "Members",
                principalColumn: "private_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInGroup_Groups_groupId",
                table: "MemberInGroup",
                column: "groupId",
                principalTable: "Groups",
                principalColumn: "group_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_Members_ownerprivate_id",
                table: "Shares",
                column: "ownerprivate_id",
                principalTable: "Members",
                principalColumn: "private_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_Groups_to_groupgroup_id",
                table: "Shares",
                column: "to_groupgroup_id",
                principalTable: "Groups",
                principalColumn: "group_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Organizations_organizationprivate_id",
                table: "Groups",
                column: "organizationprivate_id",
                principalTable: "Organizations",
                principalColumn: "private_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Members_ownerprivate_id",
                table: "Groups",
                column: "ownerprivate_id",
                principalTable: "Members",
                principalColumn: "private_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_Members_ownerprivate_id",
                table: "Organizations",
                column: "ownerprivate_id",
                principalTable: "Members",
                principalColumn: "private_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Organizations_organizationprivate_id",
                table: "Members");

            migrationBuilder.DropTable(
                name: "MemberInGroup");

            migrationBuilder.DropTable(
                name: "Shares");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
