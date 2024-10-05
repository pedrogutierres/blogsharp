﻿
using Blog.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blog.Data.Helpers
{
    public static class DbMigrationHelperExtension
    {
        public static void UseDbMigrationHelper(this WebApplication app)
        {
            DbMigrationHelpers.EnsureSeedData(app).Wait();
        }
    }

    public static class DbMigrationHelpers
    {
        public static async Task EnsureSeedData(WebApplication serviceScope)
        {
            var services = serviceScope.Services.CreateScope().ServiceProvider;
            await EnsureSeedData(services);
        }

        public static async Task EnsureSeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (env.IsDevelopment())
            {
                await context.Database.MigrateAsync();

                await EnsureSeedProducts(scope, context);
            }
        }

        private static async Task EnsureSeedProducts(IServiceScope scope, ApplicationDbContext context)
        {
            if (await context.Users.AnyAsync())
                return;

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin@blogsharp.com",
                NormalizedUserName = "ADMIN@BLOGSHARP.COM",
                Email = "ADMIN@BLOGSHARP.COM",
                NormalizedEmail = "ADMIN@BLOGSHARP.COM",
                AccessFailedCount = 0,
                LockoutEnabled = false,
                TwoFactorEnabled = false,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(user, "Blog@123456");

            if (!result.Succeeded)
                return;

            if (!await roleManager.RoleExistsAsync("Administrador"))
            {
                var role = new IdentityRole();
                role.Name = "Administrador";
                await roleManager.CreateAsync(role);
            }

            await userManager.AddToRoleAsync(user, "Administrador");

            var autor = new Autor
            {
                Id = Guid.Parse(user.Id),
                Nome = "Administrador",
                Sobrenome = "BlogSharp"
            };

            context.Autores.Add(autor);
            await context.SaveChangesAsync();

            await context.Posts.AddAsync(new Post
            {
                Id = Guid.NewGuid(),
                AutorId = autor.Id,
                Titulo = "Ubisoft será comprada pela Tencent? Rumores esquentam",
                Conteudo = "<p>A maior bomba em potencial do dia partiu de <a href=\"https://www.bloomberg.com/news/articles/2024-10-04/tencent-guillemot-family-are-said-to-consider-buyout-of-ubisoft\" rel=\"noopener noreferrer\" target=\"_blank\">reportagem do site Bloomberg</a>, onde foi dito que a <strong>Ubisoft </strong>está considerando a possibilidade de ser comprada pela gigante chinesa <strong>Tencent</strong>.</p><p><br></p><p>A produtora estaria ao menos disposta a ouvir propostas, já que <a href=\"https://flowgames.gg/recepcao-star-wars-outlaws-despenca-acoes-ubisoft/\" rel=\"noopener noreferrer\" target=\"_blank\">o valor da empresa e de suas ações despencou ao longo de 2024</a> com o fiasco do lançamento de <strong>Star Wars Outlaws</strong> e as controvérsias e adiamentos de <strong>Assassin’s Creed Shadows</strong>.</p><p><br></p><h2><strong>O futuro da Ubisoft está em aberto</strong></h2><p><br></p><p>Hoje, a <strong>Guillemot Brothers Ltd. é dona de 14% das ações da Ubisoft</strong>, enquanto a <strong>Tencent possui participação de 9,99%</strong>, sendo que ambas as partes estariam conversando com consultores em busca da melhor forma de aumentar o valor da marca.</p><p><br></p><p>Acionistas minoritários como a <strong>AJ Investments</strong>, por sua vez, colocam pressão por uma aquisição privada e venda para parceiros.</p><p><br></p><p>Segundo as fontes da Bloomberg, <strong>nenhuma decisão foi tomada até a data de publicação esta matéria</strong>, mas as movimentações com ações da Ubi permanecem travadas no mercado. Novos desenvolvimentos sobre o tema podem acontecer a qualquer momento, então fique de olho aqui no portal e <a href=\"https://www.youtube.com/@FlowGamesPodcast\" rel=\"noopener noreferrer\" target=\"_blank\">no canal do Flow Games</a> para acompanhar todas as novidades.</p>",
                DataHoraCriacao = DateTime.Now
            });

            await context.Posts.AddAsync(new Post
            {
                Id = Guid.NewGuid(),
                AutorId = autor.Id,
                Titulo = "GTA Vice City Nextgen Edition está ficando ABSURDO",
                Conteudo = "<p>O <a href=\"https://flowgames.gg/gta-vice-city-roda-em-roteador-sem-pc-nem-console/\" rel=\"noopener noreferrer\" target=\"_blank\">GTA Vice City</a> é o preferido de muitos fãs da franquia. E enquanto a <a href=\"https://flowgames.gg/gta-5-rockstar-trabalha-desde-inicio-2024-pc-melhoria/\" rel=\"noopener noreferrer\" target=\"_blank\">Rockstar</a> lançou a sua versão definitiva sendo alvo de críticas, os modders provam que podem fazer um trabalho melhor.</p><p><br></p><h2><strong>GTA Vice City Nextgen Edition ganha novo vídeo</strong></h2><p><br></p><p>Anunciado no ano passado, o GTA Vice City Nextgen Edition, como o seu nome sugere, é um tipo de remake que promete deixar o clássico jogo da Rockstar com gráficos da nova geração e outras melhorias. Entretanto, o que interessa é que o <a href=\"https://www.youtube.com/@RevTeamHD\" rel=\"noopener noreferrer\" target=\"_blank\">Revolution Team</a> acaba de mostrar como está ficando sua jogabilidade, principalmente em combate, e efeitos visuais.</p><p><br></p><p>O projeto, que é feito na engine do GTA IV, usa alguns modelos do Vice City encontrados na versão Definitive Edition. Graças a essa combinação é possível notar que texturas, ambientes e outros elementos gráficos estão melhores, enquanto os modelos dos personagens ainda ficam um pouco datados.</p><p><br></p><p>O resultado de GTA Vice City Nextgen Edition, de toda forma, é incrível, principalmente quando paramos para compará-lo com o jogo original. Caso esteja na dúvida, veja abaixo como era a missão Trojan Voodoo sem as melhorias.</p><p><br></p><h2><strong>Mod sai ainda neste ano</strong></h2><p><br></p><p>De acordo com o Revolution Team, o mod contará com todas as missões originais e até mesmo as missões secundárias. Como era esperado, a modificação estará disponível em inglês, mas também em russo, devido aos seus desenvolvedores serem da Russia.</p><p><br></p><p>O GTA Vice City Nextgen Edition será liberado para download gratuitamente até o final de 2024, mas a sua data de lançamento ainda não está definida.</p>",
                DataHoraCriacao = DateTime.Now
            });

            await context.Posts.AddAsync(new Post
            {
                Id = Guid.NewGuid(),
                AutorId = autor.Id,
                Titulo = "Melhor no PS5 Pro! Diablo 4 Vessel of Hatred confirma melhorias",
                Conteudo = "<p>Aumentando a <a href=\"https://flowgames.gg/ps5-pro-lista-de-jogos-aprimorados-videogame/\" rel=\"noopener noreferrer\" target=\"_blank\">lista de jogos que rodarão aprimorados no PS5 Pro</a>, Diablo 4 Vessel of Hatred é mais um jogo oficialmente confirmado para receber melhorias.</p><p><br></p><p>O anúncio veio diretamente de Rod Fergusson, o atual chefe da franquia, <a href=\"https://twitter.com/RodFergusson/status/1841960958672928893?ref_src=twsrc%5Etfw\" rel=\"noopener noreferrer\" target=\"_blank\">na saudosa rede social X</a>, ainda banida no Brasil. Entenda a seguir.</p><p><br></p><h2><strong>Diablo 4 Vessel of Hatred terá melhorias no PS5 Pro</strong></h2><p><br></p><p>“Me perguntaram bastante isso recentemente, e fico feliz em anunciar que Diablo 4 e Vessel of Hatred de fato serão melhorados no PS5 Pro”, celebrou Rod. “Estou orgulhoso do nosso time que trabalhou duro para isso acontecer. Anunciaremos mais detalhes em breve.”</p><p><br></p><p>Com versões para PC, PS4, PS4, Xbox Series X/S e Xbox One, <strong>a expansão será lançada já no próximo dia 8 de outubro</strong>, mas o patch com melhorias do PS5 Pro ainda não tem uma data exata para ser disponibilizado.</p><p><br></p><p>O que você achou desse anúncio? Comente a seguir!</p>",
                DataHoraCriacao = DateTime.Now
            });

            await context.Posts.AddAsync(new Post
            {
                Id = Guid.NewGuid(),
                AutorId = autor.Id,
                Titulo = "Review Silent Hill 2 Remake: perdido em tradução, achado em poesia",
                Conteudo = "<p>Silent Hill 2 Remake tinha uma tarefa árdua desde sua concepção. Alguns autores durante os séculos disseram de diferentes maneiras que toda tradução é uma traição. Não quero soar presunçoso e cravar que algumas obras são intocáveis, mas é inegável que Silent Hill 2 era uma das mais difíceis de mexer: modifique algumas coisas e ele perderia sua identidade.</p><p><br></p><p>Esse é um ponto interessante. Algumas coisas existem em um lugar e tempo que os astros simplesmente se alinham. O elenco perfeito, as condições perfeitas, a ideia fantástica, tudo operando em uma sinergia sem igual. Como alguém simplesmente recria um sentimento? Boa sorte se alguém quiser algum dia refazer a trilogia original de Star Wars ou O Senhor dos Anéis – ou qualquer outro clássico, como “O Poderoso Chefão”.</p><p><br></p><p>Não quero ser preciosista com Silent Hill 2, mas algumas coisas são simplesmente muito únicas. Ele é datado hoje, mas também é fruto de sua época: modernizar era um risco, mexer pouco não justificaria um remake. Criado pela Team Silent, uma lendária equipe de desenvolvedores da Konami, o game tem uma qualidade ímpar, criou novos parâmetros no terror psicológico e reuniu a sinergia de pessoas brilhantes que criaram algo atemporal. Bom, não tanto.</p><p><br></p><p>Também é inegável que Silent Hill 2 é um produto de seu tempo, datado em combate, exploração limitada e algumas mecânicas dos anos 2000 que não se sustentam hoje. E, talvez, sempre seria. Silent Hill 2 Remake, assim como outras reimaginações que vemos por aí, queria unir a modernização de um clássico e entregar o impacto e sentimento do original.</p><p><br></p><p>Eu fui cético desde o começo e, para ser honesto, não gostei da maioria dos materiais de divulgação iniciais. Apesar de os últimos trailers me animarem bastante, ainda estava com um pé atrás. Afinal, são poucos jogos com esse status tão atemporal. Mas, com muita alegria, posso falar que a Bloober Team calou a minha boca e me provou redondamente enganado.</p><p><br></p><p>Silent Hill 2 Remake é uma modernização digna de entrar no hall da fama dos remakes. E, se querem uma opinião ainda mais ousada, fiquem com essa: <strong>Silent Hill 2 Remake é, em todos os aspectos, melhor que o original. Todos</strong>. A mistura entre a fidelidade extrema, material inédito e remixagem do conteúdo original cria um prato cheio para novatos e até mesmo os fãs mais fervorosos (e apostaria dinheiro nisso).</p><p><br></p><p>Se você ficou curioso, venha ver a nossa análise completa!</p><p><br></p><h2><strong>História intacta, mas com um temperinho</strong></h2><p><br></p><p>Para ser justo e não depender de memórias potencialmente falsas, eu zerei novamente Silent Hill 2 dias antes de começar Silent Hill 2 Remake. Afinal, apesar de ser fissurado no jogo original e ter zerado algumas vezes na adolescência, algumas coisas se perdem com o tempo e a visão dos anos 2000 é bem diferente da de agora.</p><p><br></p><p>Ao zerar Silent Hill 2 Remake, é muito interessante ver que a história foi mantida de maneira praticamente intacta, já que esse era possivelmente o elemento mais delicado. Mas quando digo intacto, é realmente intacto: literalmente os mesmos diálogos, palavra por palavra, foram reproduzidos, apesar de ter uma atuação bem mais moderna e condizente com o aspecto realista de agora (que é muito boa, por sinal).</p><p><br></p><p>O enredo segue ao pé da letra toda a jornada de James Sunderland atrás de Mary, que, supostamente, enviou uma carta para que ele a encontrasse em Silent Hill três anos após sua morte. Durante a campanha, Laura, Angela, Maria, Eddie continuam presentes, os temas continuam intactos e todas as sutilezas de temas psicológicos são abordadas com maestria (e até melhor, para ser sincero).</p><p><br></p><p>Se você é um fã das antigas, não se preocupe, aqui está tudo seguro. Porém, não quer dizer também que você não possa esperar nada novo. A Bloober Team se inspirou nos moldes da Capcom e fez alguns “remixes”, mudando alguns acontecimentos de lugar, trazendo novas áreas e expandindo bastante do material original – e ainda traz novos finais, mas não vou comentar nada sobre eles para evitar spoilers.</p><p><br></p><p>O time foi tão apaixonado que, mesmo nos pontos que as coisas mudaram, eles adicionaram um colecionável de deja vu, ou seja, uma forma de mostrar que o material antigo deu as caras, mas não deve ser a progressão natural dessa vez. Coisas como a área original da primeira luta contra Pyramid Head, a entrada da boate, enigmas, locais, itens utilizados em puzzles antigos e mais. É realmente muito legal e sacia o desejo do fã fervoroso.</p><p><br></p><p>As cutscenes clássicas estão todas em SIlent Hill 2 Remake (às vezes até com a mesma direção), mas algumas delas acontecem em lugares distintos e outras levam para algumas surpresas muito bem-vindas, tudo sem mudar absolutamente nada do contexto original. E há até algumas adições, que exploram de uma maneira mais explícita elementos que ficaram “escondidos” em arquivos, tirando a chance de perder alguma nuance.</p><p><br></p><p>Mas fique tranquilo, nada disso fere o delicado e tão sutil contexto do original. Silent Hill 2 Remake consegue contar a mesma história com uma qualidade de altíssimo nível, algo que nem mesmo a Capcom soube acertar na sua nova onda de remakes.</p><p><br></p><p>Portanto, apesar de um fã de carteirinha saber até mesmo as falas que vão acontecer, os personagens que vão aparecer e os resultados dessas cenas, há espaço para surpresas e isso é ótimo (até porque nada é desrespeitado ou fere o “cânone”). É legal ver a trama intacta, claro, mas se ela fosse recontada 100% igual, talvez careceria de um frescor para incentivar essa revisita.</p><p><br></p><p>Pode parecer básico e pouco ambicioso não tocar tanto neste aspecto, mas a equipe sabia que esse era um dos elementos imutáveis para manter a mensagem. Particularmente neste caso, essa era a abordagem correta, mesmo que a troco de não ter o mesmo impacto do original por já sabemos exatamente todos os eventos. Apesar de parecer ruim, não me entenda mal. O impacto emocional ainda é gigantesco, só perdemos um pouco do fator novidade.</p><p><br></p><p>Todo o subtexto, atmosfera, sutilezas e mensagens são preservadas de uma forma impressionante, mas com uma apresentação melhor e mais clara, então não há nada além de elogios por aqui. Mas, felizmente, outros elementos que estavam datados também tiveram sua dose cavalar de atenção.</p><p><br></p><h2><strong>Um remake que expande bastante</strong></h2><p><br></p><p>Silent Hill 2 podia parecer um jogo longo, mas, na verdade, não tem uma campanha maior que 6 horas. Já Silent Hill 2 Remake apresenta uma campanha de mais de 13 horas (segundo o jogo, mas a Steam marcou 15hh30 para mim) sem encheção de linguiça. Como? Aumentando todo o mapa, seja trazendo áreas inéditas ou expandindo o que já existia.</p><p><br></p><p>Alguns lugares, como os Apartamentos de Wood Side, Hospital Brookhaven, Prisão Toluca e outros, foram repaginados e ganharam maior extensão para a exploração, seja com novos chefes (sim!), puzzles mais complexos e que requerem mais backtracking ou até mesmo a busca por recursos: afinal, há mais áreas que você vai querer fuçar por recursos ou colecionáveis.</p><p><br></p><p>Além disso, muitos cenários antes inacessíveis (aqueles que conhecíamos, mas nunca pudemos entrar) agora são exploráveis, como a floricultura, a loja de discos, o Happy Burger, o estacionamento na frente do motel e muito mais. Se você conhece Silent Hill 2 de cabo a rabo, vai identificar rapidamente o que há de novo. A Bloober Team teve muito cuidado e respeito aqui, mas não se prendeu a replicar tudo.</p><p><br></p><p>Certamente, eu não estaria elogiando tudo isso se não fizesse sentido. O game original era muito bom, claro, mas é evidente que há diversos lugares que chamavam atenção e não tinham nada para aproveitar. Agora, além de oferecer novas áreas, todos os cenários foram expandidos e sempre há alguma coisa para encontrar. Sem contar o óbvio, claro: não há mais telas de loading e tudo roda de maneira fluida.</p><p><br></p><p>Além dos colecionáveis e arquivos, há muitos recursos para quem buscar minuciosamente. Inclusive, a Bloober Team criou uma mecânica para que os jogadores quebrem vidros dos carros e prateleiras para pegar itens, então sempre há alguma coisa para encontrar se você tiver o olho afiado.</p><p><br></p><p>Como há mais ambientes internos para explorar também, armários e gavetas frequentemente trazem algo, bem nos moldes de The Last of Us. Apesar de parecer maçante, os devs dosaram muito bem as sequências de história, exploração e combate, então nunca é entediante andar por aí.</p><p><br></p><p>Outro ponto interessante é que algumas paredes e janelas também podem ser quebradas e James pode pular ou passar por esses obstáculos. Há sempre alguma fresta, vão ou buraco para se esgueirar e isso é facilmente visualizado no cenário sem a necessidade da famosa “tinta amarela”.</p><p><br></p><p>E claro, temos os puzzles para adicionar variedade nessa equação. Assim como no original, Silent Hill 2 Remake traz uma dificuldade selecionável para os desafios e combate de forma separada, então tudo depende do quanto você quer malhar a mente. No Normal, eu achei os puzzles muito inteligentes e divertidos, sem segurar na mão do jogador, mas eliminando alguns ruídos e confusões que o game original tinha.</p><p><br></p><p>Você vai encontrar uma mistura de puzzles bem similares ao conteúdo de 2001 (mas com resoluções diferentes), enigmas inéditos e até algumas progressões idênticas, mas com itens em locais distintos, sempre mesclando a fidelidade, remixagem e material novo. Essa mistura dá certa do começo ao fim e não poderia elogiar mais.</p><p><br></p><p><strong>Os puzzles conseguem honrar o material de origem, seja em colecionáveis deja vu, em soluções iguais (mas com elementos diferentes), ou algo totalmente inédito, sempre premiando um backtracking maior e enigmas inteligentes que não subestimam o jogador</strong></p><p><br></p><p>Outro elemento que quero enaltecer bastante em Silent Hill 2 Remake é o Otherworld. Se você conhece a franquia, certamente sabe que se trata do mundo distorcido, quase como o “mundo invertido” em comparação à cultura pop moderna.</p><p><br></p><p>Sua memória pode lhe enganar, mas Silent Hill 2 trazia o mundo invertido aterrorizante em apenas duas partes: no hospital e uma leve passagem no hotel do fim da campanha (lembre-se: esse era o segundo título, alguns elementos ainda não eram marca registrada). A Bloober Team expandiu esse conceito e soube explorar esse imaginário da série, trazendo mais dessa icônica transformação do mundo para outros locais do game.</p><p><br></p><p>Vamos pegar os Apartamentos de Woodside, por exemplo: aqui, os dois prédios foram unificados em um único complexo (bem maiores, por sinal) e isso por si só já é bem legal em termos de expansão, mas em vez de trazer um segundo edifício, agora ele é uma entrada para o Otherworld. Isso se repete em alguns lugares da campanha e é incrível o trabalho da Bloober Team para criar essas novas versões macabras.</p><p><br></p><p>Inclusive, em vez de trazer uma forma mais abrupta (como a sirene tocando ou uma mecânica para transitar entre os mundos), o Otherworld de Silent Hill 2 Remake sempre vem aos poucos, com mudanças progressivas no cenário conforme a história se desenrola. Essa é uma das melhores adições, porque há muitos lugares que gritavam por uma versão distorcida e, no fim, isso ainda traz mais conteúdo.</p><p><br></p><p>No geral, Silent Hill 2 Remake amplica muita coisa e traz uma campanha digna singleplayer digna da atualidade, mas em momento algum peca em relação ao material original. É o tipo de remake que os fãs mais querem: fidedigno e cheio de novidades que fazem sentido serem expandidas.</p><p><br></p><h2><strong>Mais terror do que nunca (se prepare)</strong></h2><p><br></p><p>Falar sobre Silent Hill 2 Remake e o game de 2001 é um passeio de volta às memórias da era PS2. Não é à toa que o título original é tido como um dos melhores jogos de terror de todos os tempos, mas, novamente, as lembranças podem enganar. É um título de horror, claro, mas era muito psicológico e contextual, sem necessariamente “dar medo”, sabe?</p><p><br></p><p>Toda a campanha era uma reflexão dos desejos, memórias reprimidas e ações de James Sunderland, mas encaixava tudo isso em ambientes de pouca visibilidade e em eventuais corredores escuros e claustrofóbicos. Mas medo? Nunca foi um jogo de causar uma tensão gigantesca – mas na época ele tinha seu impacto.</p><p><br></p><p>Silent Hill 2 Remake vira esse conceito de ponta cabeça. A Bloober Team soube aproveitar o material original e acentuar todas as características de horror em 200%, seja pelo trabalho visual, sonoro ou até ambientação – e algumas surpresinhas aqui e ali.</p><p><br></p><p>Diversas seções da campanha são realmente tensas, daquelas de dar um pause e pegar uma água para respirar. Tudo isso é sem abusar de jump scares que, dependendo do ponto de vista, há pouquíssimos – e o que existem honram o original ou são feitos de maneira excelente. Existem sim alguns sustos (que explico melhor na parte de gameplay), mas a tensão ocorre pela atmosfera e não por truques baratos.</p><p><br></p><p>Seja em Otherworld ou na calmaria diurna de Silent Hill 2 Remake, há um clima muito aterrorizante em certas partes, especialmente pela construção de clímax que você sabe que vão acontecer. Outro ponto que ajuda bastante é a decisão criativa de trazer gráficos realistas e um uso absurdamente bom de luz e sombras.</p><p><br></p><p>Os monstros reagem e interferem na sua lanterna, que pode falhar em alguns momentos e deixá-lo em um breu total, apenas com a ciência de há companhia das mais agonizantes e barulhos de arrepiar os pelos dos braços. Além disso, como citado, ter o Otherworld em mais ocasiões ajudou a equipe a liberar a imaginação e criar locais que, sem dúvidas, são frutos dos piores pesadelos da humanidade.</p><p><br></p><p>Entretanto, é o design de som que leva o prêmio. A Bloober Team realizou um trabalho espetacular na música ambiente certa e nos efeitos extremamente bem-feitos, criando momentos de antecipação e tensão constante. <strong>Jogue Silent Hill 2 Remake de fones, se possível.</strong></p><p><br></p><h2><strong>Combate refeito se encaixa perfeitamente</strong></h2><p><br></p><p>A história era um ponto delicado de mexer e a Bloober Team optou pelo seguro – e se saiu muito bem. Mas e o combate? Silent Hill 2 Remake não poderia oferecer muitos confrontos ou algo complexo a ponto de virar um shooter, mas evitar as lutas também cairia em algo próximo de um walking simulator (e que definitivamente não faria jus ao original ou aos moldes modernos).</p><p><br></p><p>Particularmente, esse era o ponto mais sensível para mim e o que mais trazia preocupações. O que vemos em Silent Hill 2 Remake não é nada mirabolante, mas sim uma solução simples e elegante: trazer os sistemas já conhecidos de câmera sob o ombro de Resident Evil, The Last of Us e até Alan Wake 2.</p><p><br></p><p>Claro, não dá para reduzir todo o trabalho a apenas isso e há muitas decisões de game design que dão suporte ao alicerce do sistema de luta refeito, mas já entro em detalhes. Na prática, temos comandos de mirar e atirar bem eficientes e que não são estranhos a ninguém.</p><p><br></p><p>Já no combate corpo a corpo, as coisas mudam um pouco. O botão de atirar se torna o comando de ataque e há um botão para esquiva, além de uma mira automática entre os adversários que funciona surpreendentemente bem para a porradaria, já que depende da direção da câmera para grudar com um ímã a atenção da batalha</p><p><br></p><p>Outro ponto interessante é que você não depende mais de menus para trocar de armas! Com atalhos no D-Pad e um simples pressionar do botão de ataque para usar golpes melee, é muito fácil alternar entre gastar balas e partir para a briga mano a mano.</p><p><br></p><p>Ok, mas… e como isso não cai na pergunta que inicia esse tópico? Como que essas adições tão modernas de shooters não tornam Silent Hill 2 Remake em algo centrado em ação?</p><p><br></p><p>Aqui vem a parte legal que a Bloober Team fez para contornar esse problema em Silent Hill 2 Remake: o comportamento de todos os monstros foi refeito do zero; não traduzido, mas recriado e recontextualizado. Também vale um outro lembrete: se você acha que o título original não tinha muitas lutas, você se engana: há muitas criaturas por toda a extensão da campanha e bastante combate.</p><p><br></p><p>Mas retornando ao tema, os inimigos apresentam comportamentos extremamente agressivos, aterrorizantes e, em alguns momentos, até imprevisíveis. Os monstrengos mais comuns podem ser fáceis de lidar individualmente, mas agora eles rastejam mais rápido, avançam e cospem ácido à distância, por exemplo. E em grupo? Se prepare para passar sufoco, especialmente em locais fechados.</p><p><br></p><p>Os manequins também ganharam uma repaginada incrível em Silent Hill 2 Remake! Agora, além de também serem extremamente agressivos, eles criam emboscadas – e aí vem a explicação para os possíveis jump scares. Entretanto, o seu rádio está sempre ligado e pode detectar que há algo suspeito por perto, então tudo depende da sua dedução para levar ou não um susto.</p><p><br></p><p>Como uma maneira de contornar isso, você tem algumas opções na manga. Por exemplo, andar devagar e desligar a lanterna faz com que as criaturas se tornem menos reativas, permitindo ataques mais poderosos que colocam os adversários no chão e ajudam o protagonista a finalizar as criaturas, mas… boa sorte andando por aí sem a lanterna acesa.</p><p><br></p><p>Outra opção é usar as armas de fogo. Claro, são extremamente efetivas, mas ainda é um survival horror e os recursos podem ser escassos. Em alguns momentos, eu tive bastante munição jogando na dificuldade Normal, mas elas eram gastas rapidamente em chefões, então cuidado com a falsa sensação de segurança – mas jogadores que gostam do gênero vão saber administrar muito bem.</p><p><br></p><p>Eu poderia falar mais dezenas de parágrafos sobre todas essas mudanças, comentar sobre a adaptação genial das enfermeiras, os monstros alternativos de Otherworld, as criaturas que surpreendem na escuridão, os chefes novos e… vou parar por aqui, porque se sentir familiar com o jogo original é algo que não serve para nada e não quero dar spoilers. Em gameplay, Silent Hill 2 é recheadíssimo das mais gratas surpresas.</p><p><br></p><p>Tudo é fantástico e isso ficou claro. Porém, há um único ponto que eu gostaria de ver melhor utilizado aqui: seria interessante se James tivesse uma barra de vigor para ataques corpo a corpo, mesmo que invisível, para evitar abusar dessa mecânica.</p><p><br></p><p>Dessa maneira, criar um risco para o confronto direto premiaria saber a hora de fugir de algumas batalhas. Ao jogar, você vai notar que masterizar esse sistema torna a experiência em um passeio no parque, então seria interessante punir ou diminuir a eficiência do uso extensivo dos ataques corporais. Assim, a tensão seria ainda maior.</p><p><br></p><p>Em suma, Silent Hill 2 Remake não reinventa a roda e aposta no simples, usa todas as cartas da mesa para trazer um sistema de luta balanceado, extremamente prazeroso, refinado e que não descaracteriza o sentimento de terror do original, mas sim o fortalece. É difícil explicar como tudo isso não quebra a ilusão do horror, mas acredite em mim: o terror nunca foi tão grande.</p><p><br></p><h2><strong>Apresentação visual soberba coroa a experiência</strong></h2><p><br></p><p>Se há algo que chamou atenção desde o anúncio de Silent Hill 2 Remake, certamente foram seus gráficos de ponta. Claro, remakes não são apenas sobre bons visuais, especialmente em casos como esse, que abordam terrores psicológicos e cada detalhe tem seu peso, cada elemento tem seu significado e cada decisão tem uma mensagem.</p><p><br></p><p>Porém, é inegável que Silent Hill 2 Remake é o crème de la crème quando se trata de tecnologia gráfica. O game utiliza a Unreal Engine 5 em todo o seu potencial e abusa de cada tecnologia de nova geração. E chegamos ao segundo ponto mais importante: a ambientação e atmosfera.</p><p><br></p><p>Se você gosta da série, sabe que existe algo muito além de uma boa direção de arte. A franquia Silent Hill tem designs de monstros, cenários e personagens muito únicos? Sim, mas é a combinação de uma combinação de fatores que cria uma atmosfera e uma ambientação inigualável. E não é uma hipérbole, é realmente algo que só Silent Hill fez até hoje, misturando uma sensação de horror, pesadelo absoluto e, por mais contraditório que seja, conforto também.</p><p><br></p><p>A luz natural refletida em ambientes escuros traz um aconchego em meio aos terrores, a névoa traz uma sensação de desconforto, a arquitetura traz um mistério. Replicar isso não era fácil e não bastava uma boa direção de arte da Bloober Team: era preciso acertar em cheio. E, em cheio, acertaram.</p><p><br></p><p>Eu não quero dar muitos detalhes para evitar spoilers, mas as sessões em que eventos mudam Silent Hill são de brilhar os olhos, com efeitos climáticos tenebrosos e que mostram que há mais de uma maneira de causar terror nos jogadores.</p><p><br></p><p>Tudo é replicado extremamente bem e faz algo que jamais pensei: recriar sentimentos que só a Team Silent fez até hoje. A Bloober Team soube trabalhar muito bem para unir a direção fantástica e os visuais de ponta. A névoa parece palpável de tão densa, a iluminação global cria um clima pesado no jogo de luz e sombras, o ray tracing traz vida ao cenário tão monocromático.</p><p><br></p><p>A cidade realmente parece o conceito de 2001 tirado do papel e exposto na realidade de tão bom são os gráficos e direção de arte. Silent Hill 2 Remake traduz, de uma forma que parecia impossível, todo o clima do original. Palmas para a Bloober Team.</p><p><br></p><p>Porém, a Unreal Engine 5 é uma faca de dois gumes. Apesar de sua popularidade, ela tem sua dose conhecida de problemas (que aparecem aqui) e também tem seu custo: Silent Hill 2 Remake é bem pesado de rodar. No PC, com uma GeForce RTX 4090, um Intel i7-13700K e 32 GB de RAM, ter tudo no Ultra e rodar em 1440p me trouxe performances abaixo do que eu esperava.</p><p><br></p><p>No geral, Silent Hill 2 Remake rodou na casa dos 80 a 90 fps, com eventuais áreas acima disso e, nas cenas mais pesadas, quase caindo para baixo dos 60 fps. Tudo isso foi testado no DLSS Qualidade e, vale lembrar, em 1440p: sequer era em 4K. Eu não sei dizer por ora como PCs mais modestos vão lidar com a experiência ou como a performance estará no PS5, mas fiquem cientes de que há margem para desempenho ruim.</p><p><br></p><h2><strong>Alguns deslizes pequenos, mas sem ofuscar a experiência</strong></h2><p><br></p><p>Diferente do que eu faço em análises, não pesei prós e contras dentro de seus próprios tópicos por um motivo:<strong> é até difícil achar defeitos em Silent Hill 2 Remak</strong>e. Entretanto, separei essa seção para comentar sobre alguns defeitos menores e elementos que não achei tão bons.</p><p><br></p><p>Já que estava comentando sobre os gráficos, vamos começar por eles. Silent Hill 2 Remake tem alguns probleminhas da Unreal Engine 5, como engasgos em alguns momentos, mas difícil dizer por ora se é por otimização, shader compilation ou carregamento de materiais durante loardings de algumas áreas.</p><p><br></p><p>Além disso, existem glitches nos visuais em alguns momentos pontuais: os flashes brancos da Unreal Engine 5 dão as caras em certos momentos, os upscalers de imagem não estão funcionando tão bem nos semblantes de alguns personagens e, curiosamente, as cutscenes têm dois problemas bem específicos.</p><p><br></p><p>O primeiro deles é que, por algum motivo, certas mechas de cabelo se tornam extremamente pixeladas em cutscenes específicas. Pode parecer besteira, mas estou falando em algo no estilo “linha no MS Paint de 1998”, sem brincadeiras. Certamente é um bug, talvez o DLSS não esteja ativado em algumas cinemáticas, realmente não sei. E, o segundo, é que as cutscenes são travadas em 30 fps (e algumas parecem estar abaixo disso, inclusive).</p><p><br></p><p>Outro ponto de Silent Hill 2 Remake que me incomodou de leve é que, apesar de ser muito redondo e praticamente sem bugs, há alguns probleminhas pontuais. Algumas animações não foram finalizadas e os personagens resetaram a uma pose padrão durante uma conversa e, em certo momento em que procurava por Laura, mesmo após encontrá-la, James ainda gritava por ela caso eu voltasse a uma área anterior.</p><p><br></p><p>É achar pelo em ovo, certamente, e tudo isso é facilmente corrigido em atualizações no futuro, especialmente porque, de forma geral, Silent Hill 2 Remake é um jogo extremamente polido neste quesito. Entretanto, há um último ponto que gostaria de tocar e que pode ser um ponto negativo.</p><p><br></p><p>Se você conhece a obra original, sabe que em versões futuras tivemos a campanha Born from a Wish, mostrando a trajetória de Maria até se encontrar com James em Rosewater Park. Ela é curtinha, mas dá um novo contexto à personagem muito interessante. Infelizmente, esse conteúdo não existe no remake.</p><p><br></p><p>Apesar de a Capcom ter criado novos parâmetros para remakes modernos, pode ser que até os parâmetros negativos tenham servido de inspiração. É difícil dizer se Born from a Wish pode chegar como um DLC no futuro, mas ao menos seria a solução “menos pior” do que simplesmente ele não existir aqui.</p><p><br></p><h2><strong>Silent Hill 2 Remake vale a pena?</strong></h2><p><br></p><p>A Bloober Team realmente se superou em Silent Hill 2 Remake. Desde o começo, quando se declararam muito fãs, tinha meu ceticismo: não é por ser fã que dá para traduzir um jogo tão único – e, agora, a equipe nunca havia entregado um jogo de qualidade excepcional, apesar de ter projetos interessantes. Talvez, por ter expectativas baixas, ser provado o contrário me deu a sensação que esse é um dos melhores remakes já feitos.</p><p><br></p><p>Sem sombra de dúvidas, Silent Hill 2 Remake faz jus ao original. Na verdade, ouso dizer que supera em absolutamente tudo, com exceção do impacto. E, honestamente? Dificilmente seria, já que mudar a história seria um pecado capital e recontar a mesma coisa sempre seria algo previsível. Contudo, sequer dá para contar isso como um contra na experiência.</p><p><br></p><p>O remake se sobressai em todos os aspectos e de todas as maneiras. Hoje, jogar o game de 2001 seria apenas para conhecer, porque o novo título entrega tudo melhorado e essa é, com certeza, a versão definitiva dessa experiência.</p><p><br></p><p>Sem dúvidas, a Bloober Team quis mexer em um verdadeiro vespeiro quando assumiu o desenvolvimento de Silent Hill 2 Remake. O balanço para traduzir essa experiência era bem difícil e poderia cair em uma armadilha: um processo criativo não se repete no copia e cola.</p><p><br></p><p>Entretanto, é gratificante terminar a campanha e ver que, dessa vez, novos parâmetros para remakes surgiram no mercado. A proporção em como a experiência mistura fidelidade altíssima, novidades e surpresas dentro do próprio material é fantástica.</p><p><br></p><p>Sim, alguns aspectos do jogo original podem ter sido perdidos na tradução, mas a arte que surgiu dessa conversão de uma linguagem arcaica é pura poesia. Novamente, Silent Hill 2 entra para os melhores jogos de terror da história.</p><p><br></p><p><strong>Silent Hill 2 Remake foi gentilmente cedido pela Konami para a realização desta análise.</strong></p><p><br></p>",
                DataHoraCriacao = DateTime.Now
            });

            await context.SaveChangesAsync();
        }
    }
}
