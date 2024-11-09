using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Data.Migrations
{
    /// <inheritdoc />
    public partial class PostImagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Autores_AutorId",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Posts_PostId",
                table: "Comentarios");

            migrationBuilder.AddColumn<byte[]>(
                name: "Imagem",
                table: "Posts",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Autores_AutorId",
                table: "Comentarios",
                column: "AutorId",
                principalTable: "Autores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Posts_PostId",
                table: "Comentarios",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Autores_AutorId",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Posts_PostId",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Posts");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Autores_AutorId",
                table: "Comentarios",
                column: "AutorId",
                principalTable: "Autores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Posts_PostId",
                table: "Comentarios",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
